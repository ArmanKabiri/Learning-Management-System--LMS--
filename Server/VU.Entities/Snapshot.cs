using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VU.Entities
{
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
        public string Serialize()
        {
            StringBuilder strSnapShot = new StringBuilder();
            strSnapShot.Append("{\"ID\":\"").Append(ID).Append("\",")
                .Append("\"CourseID\":\"").Append(CourseID).Append("\",")
                .Append("\"SequenceNumber\":\"").Append(SequenceNumber).Append("\",")
                .Append("\"Img\":\"").Append(ImgPath).Append("\",")
                .Append("\"ImgPath\":\"").Append(ImgPath).Append("\",")
                //   Append("\"VoicePath\":\"").Append(VoicePath).Append("\",")
            .Append("\"LocxMouse\":\"").Append(LocxMouse).Append("\",")
            .Append("\"LocyMouse\":\"").Append(LocyMouse).Append("\",");
            if (Voice != null)
                strSnapShot.Append("\"Voice\":\"").Append(ByteArray2String(Voice)).Append("\"");
            else
                strSnapShot.Append("\"Voice\":\"").Append("\"");

            strSnapShot.Append("}");
            return strSnapShot.ToString();
        }

        public string ByteArray2String(byte[] bytes)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
                str.Append((char)bytes[i]);
            return str.ToString();
        }
    }
}
