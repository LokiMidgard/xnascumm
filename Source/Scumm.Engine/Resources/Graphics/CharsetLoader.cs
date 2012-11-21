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

            // Read charset header information
            var unknown = reader.ReadBytes(6);

            byte[] colormap = new byte[15];
            for (int i = 0; i < 15; ++i)
                colormap[i] = reader.ReadByte();

            long initPosition = reader.BaseStream.Position;

            byte numBitsPerPixel = reader.ReadByte();
            byte fontHeight = reader.ReadByte();
            short numChars = reader.ReadInt16();

            int[] offsets = new int[numChars];
            for (int i = 0; i < numChars; ++i)
                offsets[i] = reader.ReadInt32();

            // Read each char
            for (int i = 0; i < numChars; ++i)
            {
                if (offsets[i] == 0)
                    continue;

                reader.BaseStream.Position = initPosition + offsets[i];

                charset.Chars[i].width = reader.ReadByte();
                charset.Chars[i].height = reader.ReadByte();
                charset.Chars[i].offX = reader.ReadByte();
                charset.Chars[i].offY = reader.ReadByte();
                charset.Chars[i].pic = new Texture2D(graphicsDevice, charset.Chars[i].width, charset.Chars[i].height);

                byte[] bytes = new byte[charset.Chars[i].width * charset.Chars[i].height * 4];
                for (int y = 0; y < charset.Chars[i].height; ++y)
                {
                    for (int x = 0; x < charset.Chars[i].width; ++x)
                    {
                        bytes[(y * charset.Chars[i].width + x) * 4] = 255;
                        bytes[(y * charset.Chars[i].width + x) * 4 + 1] = 0;
                        bytes[(y * charset.Chars[i].width + x) * 4 + 2] = 0;
                    }
                }

                charset.Chars[i].pic.SetData(bytes);
            }

            return charset;
        }
    }
}
