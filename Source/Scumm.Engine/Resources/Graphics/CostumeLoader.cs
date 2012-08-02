using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.Resources.Loaders;
using Scumm.Engine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scumm.Engine.Resources.Graphics
{
    class CostumeLoader : ResourceLoader
    {
        GraphicsDevice graphicsDevice;

        public CostumeLoader(GraphicsDevice device)
        {
            graphicsDevice = device;
        }

        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            Costume costume = new Costume(resourceId);

            // Read Room Header information
            if (reader.FindDataBlock("COST") == 0)
            {
                throw new InvalidOperationException("Could not find the costume header block.");
            }

            var startOffset = reader.BaseStream.Position;
            //reader.BaseStream.Position += 6;

            var animationsCount = reader.ReadByte();
            var format = reader.ReadByte();

            var paletteSize = ((format & 1) == 0) ? 16 : 32;
            var mirror = ((format & 128) == 0);

            // read palette
            var palette = new byte[paletteSize];
            for (int i = 0; i < paletteSize; ++i)
            {
                palette[i] = reader.ReadByte();
            }
            //for (int i = 0; i < 8; ++i)
            //    reader.ReadByte();

            reader.ReadUInt16();
            var offset = reader.ReadUInt16();

            // Load Test Frame
            reader.BaseStream.Position = startOffset + offset - 6;
            offset = reader.ReadUInt16();

            // Load the room palette associated with the costume
            var roomPalette = resourceManager.Load<Room>("ROOM", (byte)parameters["RoomId"]).Palette;

            reader.BaseStream.Position = startOffset + offset - 6;

            // Load a test frame
            LoadTestFrame(reader, costume, palette, roomPalette, mirror);

            return costume;
        }

        private void LoadTestFrame(ScummBinaryReader reader, Costume costume, byte[] palette, Color[] roomPalette, bool containsRedirection)
        {
            var width = reader.ReadUInt16();
            var height = reader.ReadUInt16();
            var relativeX = reader.ReadInt16();
            var relativeY = reader.ReadInt16();
            var movementX = reader.ReadInt16();
            var movementY = reader.ReadInt16();

            if (containsRedirection)
            {
                var redirectionLimb = reader.ReadByte();
                var redirectionPict = reader.ReadByte();
            }

            //  Create the texture data array
            var textureData = DecodeImageData(reader, width, height, palette, roomPalette);

            costume.TestTexture = new Microsoft.Xna.Framework.Graphics.Texture2D(graphicsDevice, width, height, false, Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color);
            costume.TestTexture.SetData(textureData);
        }

        // TODO: Move that method in the graphics namespace as a loader?
        private byte[] DecodeImageData(ScummBinaryReader reader, int width, int height, byte[] palette, Color[] roomPalette)
        {
            var imageData = new byte[width * height * 4];

            byte shift;
            byte mask;

            if (palette.Length == 16)
            {
                shift = 4;
                mask = 0xF;
            }
            else
            {
                shift = 3;
                mask = 0x7;
            }

            int x = 0;
            int y = 0;

            bool end = false;

            //while (end == false)
            for(int i = 0; i < 100; ++i)
            {
                
                var rep = reader.ReadByte();
                var colorPaletteIndex = (byte)(rep >> shift);
                Color color = Color.Black;

                var roomColor = roomPalette[palette[colorPaletteIndex]];
                color.R = roomColor.R;
                color.G = roomColor.G;
                color.B = roomColor.B;

                rep &= mask;

                if (rep == 0)
                {
                    rep = reader.ReadByte();
                }

                while (rep > 0)
                {
                    var pixelIndex = y * width * 4 + x * 4;

                    imageData[pixelIndex] = color.R;
                    imageData[pixelIndex + 1] = color.G;
                    imageData[pixelIndex + 2] = color.B;
                    imageData[pixelIndex + 3] = (colorPaletteIndex == 0) ? (byte)0 : (byte)255;

                    rep--;
                    y++;

                    if (y >= height)
                    {
                        y = 0;
                        x++;

                        if (x >= width)
                        {
                            end = true;
                        }
                    }
                }
            }

            return imageData;
        }
    }
}
