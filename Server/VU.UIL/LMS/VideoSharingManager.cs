using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Json;
using System.Net.Sockets;
using System.Text;
using System.Web;
using Microsoft.AspNet.SignalR;
using NAudio.Lame;
using NAudio.Wave;
using SilverlightSignalRTest.Web;
using VU.BLL;
using VU.BLL.LMS;
using VU.Entities;
using System.Threading;

namespace VU.UIL.LMS
{
    public class VideoSharingManager
    {
        
        Semaphore sem_udp = new Semaphore(0,100);
        Queue<byte[]> snapshotQueue = new Queue<byte[]>();

        //private delegate void SomeFunctionDelegate(string serverPath);
        //private SomeFunctionDelegate sfd;

        public VideoSharingManager(string serverPath, int courseID, int sessionNumber)
        {
            CacheFactory.Instance.GetAll<String, Snapshot>("Snapshot").RemoveAll(snp => snp.CourseID == courseID);
            new Thread(delegate()
            {
                ServerReceiveData();
            }).Start();

            new Thread(delegate()
            {
                ServerParseData(serverPath);
            }).Start();
            

            //sfd = new SomeFunctionDelegate(this.ServerReceiveData);
            //sfd.BeginInvoke(serverPath, null, null);
        }


        private void ServerParseData(string serverPath)
        {
            
            Snapshot snapShot;
            int specSize, ImgSize, VoiceSize, indexRecievedPacket, indexImg, indexVoice;
            byte[] specPacket;
            string pathVoice, pathImg;
            try
            {
                while (true)
                {
                    sem_udp.WaitOne();
                    Console.WriteLine("saving data");
                    specSize = 0; ImgSize = 0; VoiceSize = 0;
                    indexRecievedPacket = 0;

                    byte[] receiveBytes = snapshotQueue.Dequeue();

                    for (indexRecievedPacket = 0; receiveBytes[indexRecievedPacket] != 42; indexRecievedPacket++)
                        specSize = specSize * 10 + (receiveBytes[indexRecievedPacket] - 48);

                    specPacket = new byte[specSize];

                    for (int indexSpec = ++indexRecievedPacket; indexSpec < specSize + indexRecievedPacket; indexSpec++)
                        specPacket[indexSpec - indexRecievedPacket] = receiveBytes[indexSpec];

                    indexRecievedPacket += specSize;
                    snapShot = new Snapshot();
                    String specString = Encoding.ASCII.GetString(specPacket);
                    JsonTextParser parser = new JsonTextParser();
                    JsonObject obj = parser.Parse(specString);

                    foreach (JsonObject field in obj as JsonObjectCollection)
                    {
                        string name = field.Name;
                        string value = string.Empty;
                        value = (string)field.GetValue();
                        switch (name)
                        {
                            case "ID":
                                snapShot.ID = Int32.Parse(value);
                                break;
                            case "CourseID":
                                snapShot.CourseID = Int32.Parse(value);
                                break;
                            case "SequenceNumber":
                                snapShot.SequenceNumber = Int32.Parse(value);
                                break;
                            case "LocxMouse":
                                snapShot.LocxMouse = Int32.Parse(value);
                                break;
                            case "LocyMouse":
                                snapShot.LocyMouse = Int32.Parse(value);
                                break;
                        }
                    }

                    for (; receiveBytes[indexRecievedPacket] != 42; indexRecievedPacket++)
                        ImgSize = ImgSize * 10 + (receiveBytes[indexRecievedPacket] - 48);
                    snapShot.Img = new byte[ImgSize];
                    for (indexImg = ++indexRecievedPacket; indexImg < ImgSize + indexRecievedPacket; indexImg++)
                        snapShot.Img[indexImg - indexRecievedPacket] = receiveBytes[indexImg];
                    indexRecievedPacket += ImgSize;

                    for (; receiveBytes[indexRecievedPacket] != 42; indexRecievedPacket++)
                        VoiceSize = VoiceSize * 10 + (receiveBytes[indexRecievedPacket] - 48);
                    snapShot.Voice = new byte[VoiceSize];
                    for (indexVoice = ++indexRecievedPacket; indexVoice < VoiceSize + indexRecievedPacket; indexVoice++)
                        snapShot.Voice[indexVoice - indexRecievedPacket] = receiveBytes[indexVoice];

                    indexRecievedPacket += VoiceSize;
                    ConvertVoice(snapShot, serverPath, out pathVoice);
                    snapShot.VoicePath = pathVoice;
                    ConvertImage(snapShot, serverPath, out pathImg);
                    snapShot.ImgPath = pathImg;

                    string ID = snapShot.CourseID.ToString() + snapShot.SequenceNumber;
                    snapShot.Img = null;
                    if (pathVoice.Length > 3)
                    {
                        using (var retMs = new MemoryStream())
                        using (var rdr = new WaveFileReader(serverPath + pathVoice))
                        using (var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, 128))
                        {
                            rdr.CopyTo(wtr);
                            snapShot.Voice = retMs.ToArray();
                            if (pathVoice.Length > 3)
                                File.WriteAllBytes(serverPath + pathVoice.Substring(0, pathVoice.Length - 3) + "mp3", snapShot.Voice);
                            else
                                snapShot.Voice = null;
                        }
                    }

                    CacheFactory.Instance.Add("Snapshot", ID, snapShot);
                    var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub2>();
                    context.Clients.All.received("رضا", snapShot.Serialize());

                }
            }
            catch (Exception e)
            {
            }
        }

        private void ServerReceiveData()
        {
            UdpClient receivingUdpClient = new UdpClient(8888);
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                while (true)
                {
                    byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
                    snapshotQueue.Enqueue( (byte[]) receiveBytes.Clone() );
                    sem_udp.Release();
                }
                receivingUdpClient.Close();
            }
            catch (Exception e)
            {
                receivingUdpClient.Close();
            }
        }


        private void ConvertVoice(Snapshot snapshot, string serverPath, out string pathVoice)
        {
            pathVoice = "";
            if (snapshot.Voice.Length != 1)
            {
                StringBuilder strVoicePath = new StringBuilder();
                strVoicePath.Append("Datafiles/LMSFiles/OnlineSessions/")
                   .Append(snapshot.CourseID.ToString()).Append("/").Append(snapshot.SequenceNumber).Append(".wav");
                pathVoice = strVoicePath.ToString();
                WaveFormat waveFormat = new WaveFormat(8000, 16, 1);
                MemoryWaveStream memoryWaveStream = new MemoryWaveStream(snapshot.Voice, serverPath + strVoicePath.ToString(), waveFormat);
            }
        }

        private void ConvertImage(Snapshot snapshot, string serverPath, out string pathImg)
        {
            pathImg = null;
            if (snapshot.Img.Length != 1)
            {
                StringBuilder strImgPath = new StringBuilder();
                strImgPath.Append("Datafiles/LMSFiles/OnlineSessions/")
                   .Append(snapshot.CourseID.ToString()).Append("/").Append(snapshot.SequenceNumber).Append(".jpg");
                pathImg = strImgPath.ToString();
                FileStream _imageStream = new FileStream(serverPath + strImgPath.ToString(), FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(_imageStream);
                bw.Write(snapshot.Img, 0, snapshot.Img.Length);
                bw.Close();
            }
        }

    }
}
