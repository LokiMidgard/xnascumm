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
        private int bitPosition = -1;

        public ScummBinaryReader(Stream baseStream)
            : base(baseStream)
        {
            this.buffer = new byte[16];
        }

        public Byte[] ReadByteString()
        {
            Byte[] str = new Byte[16];

            int i = 0;
            Byte next = ReadByte();

            while(next != 0xFF) {
                str[i] = next;
                ++i;
                next = ReadByte();
            }

            return str;
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

        public byte ReadBit()
        {
            if (this.bitPosition > 7 || this.bitPosition == -1)
            {
                this.FillInternalBuffer(1);
                this.bitPosition = 0;
            }

            int bit = -1;
            int byteOffset = this.bitPosition;

            bit = ((this.buffer[0] >> byteOffset) & 0x01);
            this.bitPosition++;

            return (byte)bit;
        }

        public byte ReadBits(int count)
        {
            int bits = 0;

            for (int i = 0; i < count; i++)
            {
                bits |= (ReadBit() << i);
            }

            return (byte)bits;
        }

        public void ResetBitCursor()
        {
            this.bitPosition = -1;
        }

        private void FillInternalBuffer(int count)
        {
            this.BaseStream.Read(this.buffer, 0, count);
        }
    }
}
