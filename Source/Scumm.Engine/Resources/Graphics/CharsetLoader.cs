using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.Resources.Loaders;
using Scumm.Engine.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Scumm.Engine.Resources.Graphics
{
    class CharsetLoader : ResourceLoader
    {
        private GraphicsDevice graphicsDevice;

        public CharsetLoader(GraphicsDevice device)
        {
            graphicsDevice = device;
        }

        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            var charset = new Charset(resourceId);

            uint blockSize = reader.FindDataBlock("CHAR");
            if (blockSize == 0)
            {
                throw new InvalidOperationException("Could not find the script header block.");
            }

            // Load the room palette associated with the charset
            // (huge hack - I have no idea what to do when room is 0)
            if ((byte)parameters["RoomId"] == 0)
                parameters["RoomId"] = (byte)10;

            long keepPos = reader.BaseStream.Position;
            var roomPalette = this.resourceManager.Load<Room>("ROOM", (byte)parameters["RoomId"]).Palette;
            reader.BaseStream.Position = keepPos;

            // Read charset header information
            var unknown = reader.ReadBytes(6);

            byte[] colormap = new byte[16];
            colormap[0] = 0;
            for (int i = 0; i < 15; ++i)
                colormap[i+1] = reader.ReadByte();

            long initPosition = reader.BaseStream.Position;

            byte numBitsPerPixel = reader.ReadByte();
            byte bitMask = (byte)(0xFF << (8 - numBitsPerPixel));

            byte fontHeight = reader.ReadByte();
            short numChars = reader.ReadInt16();

            uint[] offsets = new uint[numChars];
            for (int i = 0; i < numChars; ++i)
                offsets[i] = reader.ReadUInt32();

            // Read each char
            for (int i = 0; i < numChars; ++i)
            {
                if (offsets[i] == 0)
                    continue;

                reader.BaseStream.Position = initPosition + offsets[i];

                charset.Chars[i].width = reader.ReadByte();
                charset.Chars[i].height = reader.ReadByte();
                // a flag is needed to disable offX
                charset.Chars[i].offX = 0; reader.ReadByte();
                charset.Chars[i].offY = reader.ReadByte();
                charset.Chars[i].pic = new Texture2D(graphicsDevice, charset.Chars[i].width, charset.Chars[i].height);

                byte[] bytes = new byte[charset.Chars[i].width * charset.Chars[i].height * 4];

                Byte bits = reader.ReadByte();
                Byte remainingBits = 8;
                for (int y = 0; y < charset.Chars[i].height; ++y)
                {
                    for (int x = 0; x < charset.Chars[i].width; ++x)
                    {
                        long colorId = (bits & bitMask) >> (8 - numBitsPerPixel);
                        long color = colormap[colorId];

                        byte alpha = 255;
                        if (colorId == 0)
                            alpha = 0;
                        
                        bytes[(y * charset.Chars[i].width + x) * 4]     = roomPalette[color].R;
                        bytes[(y * charset.Chars[i].width + x) * 4 + 1] = roomPalette[color].G;
                        bytes[(y * charset.Chars[i].width + x) * 4 + 2] = roomPalette[color].B;
                        bytes[(y * charset.Chars[i].width + x) * 4 + 3] = alpha;

                        bits = (byte)(bits << numBitsPerPixel);
                        remainingBits -= numBitsPerPixel;
                        if (remainingBits <= 0)
                        {
                            bits = reader.ReadByte();
                            remainingBits = 8;
                        }
                    }
                }

                charset.Chars[i].pic.SetData(bytes);
            }

            return charset;
        }
    }
}
