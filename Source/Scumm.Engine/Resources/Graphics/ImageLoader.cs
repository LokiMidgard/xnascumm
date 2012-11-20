using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Scumm.Engine.Resources.Loaders
{
	public class ImageLoader : ResourceLoader
	{
        GraphicsDevice graphicsDevice;
        static int imageNum = 0;

        public ImageLoader(GraphicsDevice device)
        {
            graphicsDevice = device;
        }

        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
		{
			var image = new Image((int)parameters["Width"], (int)parameters["Height"]);
			var roomPalette = (Color[]) parameters["RoomPalette"];

			// TODO: Pass in the parameters information of what of image we need to process (Background or object image)

			// Read the image header
            if (reader.FindDataBlock("RMIH", (uint)parameters["RoomOffset"]) == 0)
			{
				throw new InvalidOperationException("Could not find the room background header block.");
			}

			var zbufferCount = reader.ReadUInt16();

			// TODO: Add code to take into account multiple image blocks (object images)
            if (reader.FindDataBlock("IM00") == 0)
			{
				throw new InvalidOperationException("Could not find image block.");
			}

			ReadImageDataBlock(reader, image, roomPalette);

			return image;
		}

		private void ReadImageDataBlock(ScummBinaryReader reader, Image image, Color[] roomPalette)
		{
            if (reader.FindDataBlock("SMAP") == 0)
			{
				throw new InvalidOperationException("Could not find image data block.");
			}

			var smapStartOffset = reader.BaseStream.Position;
			var stripesCount = image.Width / 8;

			//  Create the texture data array
			var textureData = new byte[image.Width * image.Height * 4];

			var stripeOffsets = new uint[stripesCount];

			// Read the offset table
			for (int i = 0; i < stripesCount; i++)
			{
				var offset = reader.ReadUInt32() - 8;
				stripeOffsets[i] = offset;
			}

			// Read and decode each stripes
			for (int i = 0; i < stripesCount; i++)
			{
				reader.BaseStream.Position = smapStartOffset + stripeOffsets[i];
	
				var stripeHeader = reader.ReadByte();
                var codingShift = (byte)(stripeHeader % 10);
				var encoderType = stripeHeader / 10;

				byte[] decodedData = null;

				if (encoderType == (int)EncoderType.UnkAOpaque)
				{
					decodedData = DecodeUnkAStripe(reader, codingShift, image.Height, roomPalette);
				}
				else if (encoderType == (int)EncoderType.UnkBOpaque)
				{
					decodedData = DecodeUnkBStripe(reader, codingShift, image.Height, roomPalette);
				}
				else if (encoderType == (int)EncoderType.UnkCOpaque)
				{
					decodedData = DecodeUnkCStripe(reader, codingShift, image.Height, roomPalette);
				}

				if (decodedData != null)
				{
					int decodedDataIndex = 0;

					for (int j = 0; j < image.Height; j++)
					{
						for (int k = 0; k < 8; k++)
						{
							var pixelIndex = i * 32 + j * image.Width * 4 + k * 4;

							textureData[pixelIndex]     = decodedData[decodedDataIndex];
							textureData[pixelIndex + 1] = decodedData[decodedDataIndex + 1];
							textureData[pixelIndex + 2] = decodedData[decodedDataIndex + 2];
							textureData[pixelIndex + 3] = decodedData[decodedDataIndex + 3];

							decodedDataIndex += 4;
						}
					}
				}
			}

            image.Texture = new Microsoft.Xna.Framework.Graphics.Texture2D(graphicsDevice, image.Width, image.Height, false, Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color);
			image.Texture.SetData(textureData);

            image.Texture.SaveAsPng(File.Create(string.Format("DebugAnims\\Test_{0}.png", imageNum)), image.Width, image.Height);
            ++imageNum;
		}

		private byte[] DecodeUnkAStripe(ScummBinaryReader reader, byte codingShift, int stripHeight, Color[] roomPalette)
		{
			var data = new byte[32 * stripHeight];

			var color = reader.ReadBits(codingShift);
			var stripePixelsLeft = 8 * stripHeight;

			data[0] = roomPalette[color].R;
			data[1] = roomPalette[color].G;
			data[2] = roomPalette[color].B;
			data[3] = 255;

			var pixelIndex = 1;
			stripePixelsLeft--;

			while (stripePixelsLeft > 0)
			{
				var pixelsCount = 1;

				if (reader.ReadBit() > 0)
				{
					if (reader.ReadBit() == 0)
					{
						color = reader.ReadBits(codingShift);
					}

					else
					{
						var inc = (reader.ReadBits(3) - 4);

                        if (inc != 0)
                        {
                            color += (byte)inc;
                        }

                        else
                        {
                            pixelsCount = reader.ReadBits(8);
                        }
					}
				}

				for (int j = 0; j < pixelsCount; j++)
				{
					var x = pixelIndex % 8;
					var y = pixelIndex / 8;

					var pixelAddress = y * 32 + x * 4;

					data[pixelAddress]     = roomPalette[color].R;
					data[pixelAddress + 1] = roomPalette[color].G;
					data[pixelAddress + 2] = roomPalette[color].B;
					data[pixelAddress + 3] = 255;

					stripePixelsLeft--;
					pixelIndex++;

					if (stripePixelsLeft == 0)
					{
						break;
					}
				}
			}

            reader.ResetBitCursor();

			return data;
		}

		private byte[] DecodeUnkBStripe(ScummBinaryReader reader, byte codingShift, int stripHeight, Color[] roomPalette)
		{
			var data = new byte[32 * stripHeight];

            var color = (int)reader.ReadByte();// its(codingShift);
			var stripePixelsLeft = 8 * stripHeight;

			var pixelIndex = 0;
			int inc = -1;

			while (stripePixelsLeft > 0)
			{
				var x = pixelIndex % 8;
				var y = pixelIndex / 8;

				var pixelAddress = y * 32 + x * 4;

				data[pixelAddress]     = roomPalette[color].R;
				data[pixelAddress + 1] = roomPalette[color].G;
				data[pixelAddress + 2] = roomPalette[color].B;
				data[pixelAddress + 3] = 255;

				stripePixelsLeft--;
				pixelIndex++;

				if (reader.ReadBit() > 0)
				{
					if (reader.ReadBit() == 0)
					{
						color = reader.ReadBits(codingShift);
						inc = -1;
					}

					else
					{
						if (reader.ReadBit() > 0)
						{
							inc = -inc;
						}

						color += inc;
					}
				}
			}

            reader.ResetBitCursor();

			return data;
		}

		private byte[] DecodeUnkCStripe(ScummBinaryReader reader, byte codingShift, int stripHeight, Color[] roomPalette)
		{
			var data = new byte[32 * stripHeight];

			var color = (int)reader.ReadBits(codingShift);
			var stripePixelsLeft = 8 * stripHeight;

			var pixelIndex = 0;
			int inc = -1;

			while (stripePixelsLeft > 0)
			{
				var x = pixelIndex / stripHeight;
				var y = pixelIndex % stripHeight;

				var pixelAddress = y * 32 + x * 4;

				data[pixelAddress]     = roomPalette[color].R;
				data[pixelAddress + 1] = roomPalette[color].G;
				data[pixelAddress + 2] = roomPalette[color].B;
				data[pixelAddress + 3] = 255;

				stripePixelsLeft--;
				pixelIndex++;

				if (reader.ReadBit() > 0)
				{
					if (reader.ReadBit() == 0)
					{
						color = reader.ReadBits(codingShift);
						inc = -1;
					}

					else
					{
						if (reader.ReadBit() > 0)
						{
							inc = -inc;
						}

						color += inc;
					}
				}
			}

            reader.ResetBitCursor();

			return data;
		}
	}
}

