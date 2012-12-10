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
        public int bitPosition = -1;

        public ScummBinaryReader(Stream baseStream)
            : base(baseStream)
        {
            this.buffer = new byte[16];
        }

        public char[] ReadCharString()
        {
            char[] str = new char[256];

            int i = 0;
            char next = ReadChar();

            while (next != 0 && next != 65533)
            {
                str[i] = next;
                ++i;
                next = ReadChar();
            }

            return str;
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

        public override byte ReadByte()
        {
            ResetBitCursor();
            return base.ReadByte();
        }

        public override short ReadInt16()
        {
            ResetBitCursor();
            return base.ReadInt16();
        }

        public override ushort ReadUInt16()
        {
            ResetBitCursor();
            return base.ReadUInt16();
        }

        public override int ReadInt32()
        {
            ResetBitCursor();
            return base.ReadInt32();
        }

        public override uint ReadUInt32()
        {
            ResetBitCursor();
            return base.ReadUInt32();
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
            bitPosition = -1;
        }

        public uint FindDataBlock(string blockType, uint startOffset)
        {
            // Position the stream to the start offset
            BaseStream.Position = startOffset;

            return FindDataBlock(blockType);
        }

        public uint FindDataBlock(string blockType)
        {
            var currentBlockType = new string(ReadChars(4));
            var itemSize = ReadUInt32BigEndian();

            while (BaseStream.Position <= BaseStream.Length)
            {
                if (currentBlockType == blockType)
                {
                    return itemSize;
                }

                if (!IsContainerBlock(currentBlockType))
                {
                    BaseStream.Position += itemSize - 8;
                }

                currentBlockType = new string(ReadChars(4));
                itemSize = ReadUInt32BigEndian();
            }

            return 0;
        }

        public uint FindDataBlockNoInfo(string blockType)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();

            byte[] byteArray = new byte[4];
            byteArray = ReadBytes(4);

            var currentBlockType = enc.GetString(byteArray);
            UInt32 itemSize = 0;

            while (BaseStream.Position <= BaseStream.Length)
            {
                if (currentBlockType == blockType)
                {
                    itemSize = ReadUInt32BigEndian();
                    return itemSize;
                }

                if (IsKnownBlock(currentBlockType))
                {
                    itemSize = ReadUInt32BigEndian();
                    BaseStream.Position += itemSize - 8;
                    byteArray = ReadBytes(4);
                    currentBlockType = enc.GetString(byteArray);
                }
                else
                {
                    byte newChar = ReadByte();
                    string newStr = "";
                    newStr += currentBlockType[1];
                    newStr += currentBlockType[2];
                    newStr += currentBlockType[3];
                    newStr += (char)newChar;
                    currentBlockType = newStr;
                }
            }
            return 0;
        }

        #region Private Methods

        private bool IsContainerBlock(string blockType)
        {
            if (blockType == "LECF" || blockType == "LFLF" || blockType == "ROOM" || blockType == "PALS" || blockType == "WRAP" || blockType == "RMIM" || blockType == "IM00" || blockType == "OBIM" || blockType == "IMnn" || blockType == "OBCD" || blockType == "SOUN" || blockType == "SOU")
            {
                return true;
            }

            return false;
        }

        private bool IsKnownBlock(string blockType)
        {
            if (blockType == "ZP01" || blockType == "ZP02")
            {
                return true;
            }

            return false;
        }

        private void FillInternalBuffer(int count)
        {
            BaseStream.Read(this.buffer, 0, count);
        }

        #endregion

    }
}
