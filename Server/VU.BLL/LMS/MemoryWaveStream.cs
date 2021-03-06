﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace VU.BLL.LMS
{
    public class MemoryWaveStream : Stream
    {
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return false; } }
        public override bool CanRead { get { return true; } }
        public override long Length { get { return _waveStream.Length; } }
        public override long Position { get { return _waveStream.Position; } set { _waveStream.Position = value; } }

        private FileStream _waveStream;

        public MemoryWaveStream(byte[] sampleData, string path, WaveFormat waveFormat)
        {
            _waveStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(_waveStream);
            bw.Write(new char[4] { 'R', 'I', 'F', 'F' });

            int length = 36 + sampleData.Length;
            bw.Write(length);

            bw.Write(new char[8] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });

            waveFormat.Serialize(bw);
            bw.Write(new char[4] { 'd', 'a', 't', 'a' });
            bw.Write(sampleData.Length);
            bw.Write(sampleData, 0, sampleData.Length);
            _waveStream.Position = 0;
            _waveStream.Close();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _waveStream.Read(buffer, offset, count);
        }

        public override void Flush()
        {
            _waveStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _waveStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}