using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Scumm.Engine.IO
{
    public class ScummBinaryReader : BinaryReader
    {
        private byte[] buffer;

        public ScummBinaryReader(Stream baseStream)
            : base(baseStream)
        {
            this.buffer = new byte[16];
        }

        public uint ReadUInt32BigEndian()
        {
            this.FillInternalBuffer(4);

            uint num = (uint)(((this.buffer[3] | (this.buffer[2] << 8)) | (this.buffer[1] << 0x10)) | (this.buffer[0] << 0x18));
            return num;
        }

        public int ReadInt32BigEndian()
        {
            this.FillInternalBuffer(4);

            int num = (int)(((this.buffer[3] | (this.buffer[2] << 8)) | (this.buffer[1] << 0x10)) | (this.buffer[0] << 0x18));
            return num;
        }

        private void FillInternalBuffer(int count)
        {
            this.BaseStream.Read(this.buffer, 0, count);
        }
    }
}
