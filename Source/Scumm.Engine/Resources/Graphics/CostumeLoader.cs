using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.Resources.Loaders;
using Scumm.Engine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Scumm.Engine.Resources.Graphics
{
    class CostumeLoader : ResourceLoader
    {
        private GraphicsDevice graphicsDevice;

        public CostumeLoader(GraphicsDevice device)
        {
            graphicsDevice = device;
        }

        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            // TODO: If the anim is mirrored, point to the same textures and do a matrix transform while rendering the costume
            var costume = new Costume(resourceId);
            
            // Read Room Header information
            if (reader.FindDataBlock("COST") == 0)
            {
                throw new InvalidOperationException("Could not find the costume header block.");
            }

            reader.BaseStream.Position -= 6;
            var startOffset = reader.BaseStream.Position;

            var size = reader.ReadUInt32();
            var test = ((char)reader.ReadByte()).ToString() + ((char)reader.ReadByte()).ToString();

            var animationsCount = reader.ReadByte();

            var format = reader.ReadByte();
            var paletteSize = ((format & 0x7F) == 0x58 || (format & 0x7F) == 0x60) ? 16 : 32;
            var containsRedirection = ((format & 0x7E) == 0x60);
            var mirrorWestPositions = (format & 0x80) == 0;

            // TODO: Decode bit 7

            // TODO : Read the full palette
            var palette = new byte[paletteSize];

            for (int i = 0; i < paletteSize; i++)
            {
                palette[i] = reader.ReadByte();
            }

            var animationCommandOffset = reader.ReadUInt16();

            // Read limb offsets
            var limbOffsets = new ushort[16];

            for (int i = 0; i < 16; i++)
            {
                limbOffsets[i] = reader.ReadUInt16();
            }

            // Read animation offsets
            var animationOffsets = new ushort[animationsCount];

            for (int i = 0; i < animationOffsets.Length; i++)
            {
                animationOffsets[i] = reader.ReadUInt16();
            }

            // Load the room palette associated with the costume
            // (huge hack - I have no idea what to do when room is 0)
            if ((byte)parameters["RoomId"] == 0)
                parameters["RoomId"] = (byte)10;
            var roomPalette = this.resourceManager.Load<Room>("ROOM", (byte)parameters["RoomId"]).Palette;

            for (int i = 4; i < animationsCount; i++)
            {
                var animation = LoadAnimation(reader, i, startOffset, animationOffsets, limbOffsets, animationCommandOffset, palette, roomPalette, containsRedirection, (i % 4) == 0 && mirrorWestPositions);
                costume.Animations.Add(animation);
            }

            return costume;
        }

        private CostumeAnimation LoadAnimation(ScummBinaryReader reader, int animationIndex, long startOffset, ushort[] animationOffsets, ushort[] limbOffsets, ushort animationCommandOffset, byte[] palette, Color[] roomPalette, bool containsRedirection, bool mirror)
        {
            if (animationOffsets[animationIndex] == 0)
            {
                return null;
            }

            reader.BaseStream.Position = startOffset + animationOffsets[animationIndex];

            var costumeAnimation = new CostumeAnimation();
            costumeAnimation.IsMirrored = mirror;

            var currentFrameIndex = 0;
            var framesCount = 0;
            var startAnimationPosition = reader.BaseStream.Position;

            while (currentFrameIndex < framesCount || currentFrameIndex == 0)
            {
                var mask = reader.ReadUInt16();

                var costumeFrame = new CostumeFrame();
                var imageData = new LayeredImageData();

                var i = 0;

                do
                {
                    if ((mask & 0x8000) != 0)
                    {
                        var startAnimationCommandOffset = reader.ReadUInt16();

                        if (startAnimationCommandOffset != 0xFFFF)
                        {
                            var flags = reader.ReadByte();

                            var loop = flags & 0x8000;
                            var endFrame = flags & 0x7F;

                            if (currentFrameIndex == 0 && framesCount == 0)
                            {
                                framesCount = Math.Max(framesCount, endFrame) + 1;
                            }

                            var oldStreamPosition = reader.BaseStream.Position;
                            reader.BaseStream.Position = startOffset + animationCommandOffset + startAnimationCommandOffset + Math.Min(currentFrameIndex, endFrame);

                            var animationCommandValue = reader.ReadByte();

                            if (animationCommandValue == 0x71)
                            {
                                // TODO: Handle special commands (sounds, etc.)
                            }

                            else if (animationCommandValue == 0x7A)
                            {
                                // TODO: Implement start command
                            }

                            else if (animationCommandValue == 0x79)
                            {
                                // TODO: Implement stopped command
                            }

                            else
                            {
                                reader.BaseStream.Position = startOffset + limbOffsets[i] + animationCommandValue * 2;
                                var pictOffset = reader.ReadUInt16();

                                reader.BaseStream.Position = startOffset + pictOffset;

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

                                imageData.CreateLayer(width, height, new Vector2(relativeX, relativeY));

                                DecodeImageData(reader, imageData, width, height, palette, roomPalette);
                            }

                            reader.BaseStream.Position = oldStreamPosition;
                        }
                    }

                    mask = (ushort)(mask << 1);
                    i++;

                } while ((mask & 0xFFFF) != 0);

                costumeFrame.FrameType = CostumeFrameType.Frame;

                // TODO: Fill offset and movement vector

                var textureData = imageData.GetBytes();
                costumeFrame.Data = new Microsoft.Xna.Framework.Graphics.Texture2D(this.graphicsDevice, imageData.Width, imageData.Height, false, Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color);
                costumeFrame.Data.SetData(textureData);

                costumeAnimation.Frames.Add(costumeFrame);

                //if (!Directory.Exists("DebugAnims"))
                //{
                //    Directory.CreateDirectory("DebugAnims");
                //}
                //costumeFrame.Data.SaveAsPng(File.Create(string.Format("DebugAnims\\Anim{0}_{1}.png", animationIndex, currentFrameIndex)), costumeFrame.Data.Width, costumeFrame.Data.Height);

                reader.BaseStream.Position = startAnimationPosition;
                currentFrameIndex++;
            }

            return costumeAnimation;
        }

        private void DecodeImageData(ScummBinaryReader reader, LayeredImageData imageData, int width, int height, byte[] palette, Color[] roomPalette)
        {
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

            while (end == false)
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
                    if (colorPaletteIndex != 0)
                    {
                        imageData.SetPixel(x, y, color);
                    }

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
        }
    }
}
