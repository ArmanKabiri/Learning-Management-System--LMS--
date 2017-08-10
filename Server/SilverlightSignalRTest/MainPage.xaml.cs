using Microsoft.AspNet.SignalR.Client;
using System;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;

using System.Windows.Media.Imaging;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Threading;


namespace SilverlightSignalRTest
{
    public partial class MainPage : UserControl
    {
        Semaphore sem_play = new Semaphore(0,100);
        Queue<Snapshot> allsnapShots = new Queue<Snapshot>();
        HubConnection connection;
        IHubProxy hub;
        MediaElement player;
        Stream stream;

        public MainPage()
        {
            player = new MediaElement();
            //player.AutoPlay = true;
            InitializeComponent();
            Thread t = new Thread(play_Next);
            t.Start();
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            int counter = 0;
            string serverUri = new Uri(HtmlPage.Document.DocumentUri, "/").ToString();
            connection = new HubConnection(serverUri, true);
            hub = connection.CreateHubProxy("ChatHub2");
            
            //player.MediaEnded += new RoutedEventHandler(play_Next);

            hub.On<string, string>("received",
                (name, message) =>
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        Snapshot snapshot = Deserialize(message);
                        Tuple<string, string> t = new Tuple<string, string>(name, snapshot.LocyMouse.ToString());
                        lstMessages.Items.Add(t);
                        if (snapshot.ImgPath.Length > 5)
                            this.imgCenter.Source = new BitmapImage(new Uri("http://localhost:1667/" + snapshot.ImgPath, UriKind.Absolute)); ;
                        if (snapshot.Voice.Length > 0)
                        {
                            
                            allsnapShots.Enqueue(snapshot);
                            //counter++;

                            
                            
                                sem_play.Release();
                                //counter = 0;
                            
                            

                           // if (allsnapShots.Count > 0)// player.CurrentState != MediaElementState.Playing)
                           //     PlayNextTrack(null, null);

                            /*
                            FileStream fs = new FileStream("TempPlayingMedia.mp3", FileMode.Create);
                            fs.Write(snapshot.Voice, 0,  snapshot.Voice.Length);
                            fs.Flush();
                            fs.Close();
                            player.Source = new Uri("TempPlayingMedia.mp3", UriKind.Relative);
                            player.Play(); */
                        }
                        if (snapshot.LocxMouse >= 0 && snapshot.LocyMouse >= 0)
                            imgMouse.Margin = new Thickness((double)snapshot.LocxMouse, (double)snapshot.LocyMouse, 0, 0);

                    });
                });

            hub.On<string>("receivedWholePacket",
               (wholePacket) =>
               {
                   this.Dispatcher.BeginInvoke(() =>
                   {
                       Tuple<string, string> t = new Tuple<string, string>("", wholePacket);
                       lstUsers.Items.Add(t);
                   });
               });

            connection.StateChanged += connection_StateChanged;
            connection.Start();
        }

        //private void PlayNextTrack(object sender, EventArgs e)
        //{
        //    while(true){
        //        sem_play.WaitOne();
        //        Snapshot snap;
        //        stream = new MemoryStream();
        //        BinaryWriter writer = new BinaryWriter(stream);
        //        //for (int i = 0; i < allsnapShots.Count; i++)
        //        //{
        //            snap = allsnapShots.Dequeue();
        //            writer.Write(snap.Voice);
        //        //}

        //        player.SetSource(stream);
        //        player.AutoPlay = true;
        //        player.Play();
        //    }
        //}
       
        private Snapshot Deserialize(string packet)
        {
            Snapshot snapShot = new Snapshot();
            int i = 0, ID_Start, ID_End, LocY_Start, LocY_End, ImgPath_Start,
                ImgPath_End, LocX_Start, LocX_End, Voice_Start, Voice_End, SequenceNumber_End, SequenceNumber_Start;
            while (i < packet.Length)
            {
                if (packet[i] == 'I' && packet[i + 1] == 'D' && packet[i + 2] == '"' && packet[i + 3] == ':')
                {

                }
                if (packet[i] == 'I' && packet[i + 1] == 'm' && packet[i + 2] == 'g' && packet[i + 3] == 'P' && packet[i + 4] == 'a'
                    && packet[i + 5] == 't' && packet[i + 6] == 'h')
                {
                    ImgPath_Start = i + 10;
                    i += 10;
                    while (packet[i] != '"')
                    {
                        i++;
                    }
                    ImgPath_End = i;
                    snapShot.ImgPath = packet.Substring(ImgPath_Start, ImgPath_End - ImgPath_Start);
                }

                if (packet[i] == 'L' && packet[i + 1] == 'o' && packet[i + 2] == 'c' && packet[i + 3] == 'x' && packet[i + 4] == 'M'
                    && packet[i + 5] == 'o' && packet[i + 6] == 'u' && packet[i + 7] == 's' && packet[i + 8] == 'e')
                {
                    LocX_Start = i + 12;
                    i += 12;
                    while (packet[i] != '"')
                    {
                        i++;
                    }
                    LocX_End = i;
                    snapShot.LocxMouse = Int16.Parse(packet.Substring(LocX_Start, LocX_End - LocX_Start));
                }
                if (packet[i] == 'S' && packet[i + 1] == 'e' && packet[i + 2] == 'q' && packet[i + 3] == 'u' && packet[i + 4] == 'e'
                   && packet[i + 5] == 'n' && packet[i + 6] == 'c' && packet[i + 7] == 'e' && packet[i + 8] == 'N')
                {
                    SequenceNumber_Start = i + 17;
                    i += 17;
                    while (packet[i] != '"')
                    {
                        i++;
                    }
                    SequenceNumber_End = i;
                    snapShot.SequenceNumber = Int16.Parse(packet.Substring(SequenceNumber_Start, SequenceNumber_End - SequenceNumber_Start));
                }
                if (packet[i] == 'L' && packet[i + 1] == 'o' && packet[i + 2] == 'c' && packet[i + 3] == 'y' && packet[i + 4] == 'M'
                   && packet[i + 5] == 'o' && packet[i + 6] == 'u' && packet[i + 7] == 's' && packet[i + 8] == 'e')
                {
                    LocY_Start = i + 12;
                    i += 12;
                    while (packet[i] != '"')
                    {
                        i++;
                    }
                    LocY_End = i;
                    snapShot.LocyMouse = Int16.Parse(packet.Substring(LocY_Start, LocY_End - LocY_Start));
                }
                if (packet[i] == 'V' && packet[i + 1] == 'o' && packet[i + 2] == 'i' && packet[i + 3] == 'c' && packet[i + 4] == 'e')
                {
                    Voice_Start = i + 8;
                    i += 8;
                    while (packet[i] != '"')
                    {
                        i++;
                    }
                    Voice_End = packet.Length - 2;
                    snapShot.Voice = StringToByteArray(packet.Substring(Voice_Start, Voice_End - Voice_Start));
                }
                i++;
            }
            return snapShot;
        }

        public byte[] StringToByteArray(string str)
        {

            byte[] retval = new byte[str.Length];
            for (int ix = 0; ix < str.Length; ++ix)
                retval[ix] = (byte)str[ix];
            return retval;
        }

        void connection_StateChanged(StateChange obj)
        {
            if (Dispatcher.CheckAccess())
            {
                btnSend.IsEnabled = (obj.NewState == ConnectionState.Connected);
            }
            else
            {
                Dispatcher.BeginInvoke(new Action<StateChange>(connection_StateChanged), obj);
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && btnSend.IsEnabled)
            {
                SendMessage();
            }
        }

        private void SendMessage()
        {
            hub.Invoke("SendMessage", txtName.Text, txtMessage.Text);
            txtMessage.Text = "";
        }

        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (allsnapShots.Count > 0)
            {
                Snapshot snap;
                if (stream.Length > 1000000)
                    stream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(stream);
                long prevPosition = stream.Position;
                int numAdd = allsnapShots.Count;
                for (int i = 0; i < allsnapShots.Count; i++)
                {
                    snap = allsnapShots.Dequeue();
                    writer.Write(snap.Voice);

                }
                //    stream = new MemoryStream();


                //stream.Write(snap.Voice, 0, snap.Voice.Length);
                player.SetSource(stream);

                //   stream.Seek(prevPosition, SeekOrigin.Begin); 


                player.AutoPlay = true;
                player.Play();
                // 
                //  if (player.CurrentState != MediaElementState.Playing)

            }

        }

        private void player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void player_MediaOpened(object sender, RoutedEventArgs e)
        {
            // player.Position = TimeSpan.FromSeconds(numRecievedPacket);
        }

        private void play_Next()
        {
            Stream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            this.Dispatcher.BeginInvoke(delegate()
            {
                player.AutoPlay = true;
                //player.SetSource(stream);
                
                //player.Position = TimeSpan.Zero;
            });
            
            while (true)
            {
                sem_play.WaitOne();
                Snapshot snap;
                snap = allsnapShots.Dequeue();

                //writer.Seek(0, SeekOrigin.Begin);
                long p=stream.Position;
                stream.Seek(stream.Length, SeekOrigin.Begin);
                writer.Write(snap.Voice);
                stream.Seek(p, SeekOrigin.Begin);
                //Deployment.Current.Dispatcher.BeginInvoke(() =>
                //{
                //    player.SetSource(stream);
                //    player.AutoPlay = true;
                //    player.Play();
                //});

                this.Dispatcher.BeginInvoke(delegate()
                {
                    
                    TimeSpan s= player.Position;
                    
                    if (player.CurrentState != MediaElementState.Playing)
                    {
                        
                        player.SetSource(stream);
                        player.Position = s;
                    }
                    
                        //player.Position = TimeSpan.Zero;
                    //player.Play();
                });
            }
        }
    }

    public class Snapshot
    {
        public int ID { get; set; }
        public int CourseID { get; set; }
        public int SequenceNumber { get; set; }
        public byte[] Img { get; set; }
        public string ImgPath { get; set; }
        public byte[] Voice { get; set; }
        public string VoicePath { get; set; }
        public int LocxMouse { get; set; }
        public int LocyMouse { get; set; }

    }

    public static class MemoryStreamExtensions
    {
        public static void Append(this MemoryStream stream, byte value)
        {
            stream.Append(new[] { value });
        }

        public static void Append(this MemoryStream stream, byte[] values)
        {
            stream.Write(values, 0, values.Length);
        }

    }

    public class Semaphore
    {
        private object Mutex { get; set; }
        private int Count { get; set; }
        private int Max { get; set; }

        public Semaphore(int count,int max)
        {
            Mutex = new object();
            Max = max;
            this.Count = count;
        }

        public void WaitOne()
        {
            while (true)
            {
                lock (Mutex)
                {
                    if (Count >0 )
                    {
                        Count--;
                        return;
                    }
                }

                Thread.Sleep(10);
            }
        }

        public void Release()
        {
            lock (Mutex)
            {
                //if (Count > 0)
                //{
                    Count++;
                //}
            }
        }
    }











    //public class Mp3Player
    //{
    //    public MediaElement player;
    //    public Semaphore sem_play;
    //    public Queue<Snapshot> snapShots;

    //    public Mp3Player(Queue<Snapshot> allsnapShots , Semaphore sem_play)
    //    {
    //        this.sem_play = sem_play;
    //        this.snapShots = allsnapShots;
    //        player = new MediaElement();
    //        player.MediaEnded += new RoutedEventHandler(PlayNextTrack);
    //    }

    //    public void PlayNextTrack(object sender, EventArgs e)
    //    {
    //        MediaElement player2 = new MediaElement();
    //        while (true)
    //        {
    //            sem_play.WaitOne();
    //            Snapshot snap;
    //            Stream stream = new MemoryStream();
    //            BinaryWriter writer = new BinaryWriter(stream);
                
    //            snap = snapShots.Dequeue();
    //            writer.Write(snap.Voice);
                
    //            player2.SetSource(stream);
    //            player2.AutoPlay = true;
    //            player2.Play();
    //        }
    //    }
    //}

}

