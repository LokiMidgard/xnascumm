using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Scumm.Engine.Resources.Graphics
{
    public class LayeredImageData
    {
        private List<Vector2> layersOffset;
        private List<Vector2> layersSize;
        private List<byte[]> layersData;
        private int currentLayer = -1;

        public LayeredImageData()
        {
            this.layersData = new List<byte[]>();
            this.layersOffset = new List<Vector2>();
            this.layersSize = new List<Vector2>();
        }

        public int Height
        {
            get;
            private set;
        }

        public int Width
        {
            get;
            private set;
        }

        public byte[] GetBytes()
        {
            this.Width = 0;
            this.Height = 0;

            // Compute the size of the final buffer
            var lowerX = int.MaxValue;
            var lowerY = int.MaxValue;

            for (int i = 0; i < this.layersSize.Count; i++)
            {
                if (this.layersOffset[i].X < lowerX)
                {
                    lowerX = (int)this.layersOffset[i].X;
                }

                if (this.layersOffset[i].Y < lowerY)
                {
                    lowerY = (int)this.layersOffset[i].Y;
                }
            }

            lowerX = -lowerX;
            lowerY = -lowerY;

            for (int i = 0; i < this.layersSize.Count; i++)
            {
                this.layersOffset[i] = new Vector2(this.layersOffset[i].X + lowerX, this.layersOffset[i].Y + lowerY);

                if (this.layersOffset[i].X + this.layersSize[i].X > this.Width)
                {
                    this.Width = (int)this.layersOffset[i].X + (int)this.layersSize[i].X;
                }

                if (this.layersOffset[i].Y + this.layersSize[i].Y > this.Height)
                {
                    this.Height = (int)this.layersOffset[i].Y + (int)this.layersSize[i].Y;
                }
            }

            // Compose the final buffer
            var data = new byte[this.Width * this.Height * 4];

            for (int i = 0; i < this.layersData.Count; i++)
            {
                for (int y = 0; y < (int)this.layersSize[i].Y; y++)
                {
                    for (int x = 0; x < (int)this.layersSize[i].X; x++)
                    {
                        var destinationPixelIndex = ((int)this.layersOffset[i].Y + y) * this.Width * 4 + ((int)this.layersOffset[i].X + x) * 4;
                        var sourcePixelIndex = y * (int)this.layersSize[i].X * 4 + x * 4;

                        data[destinationPixelIndex] = this.layersData[i][sourcePixelIndex];
                        data[destinationPixelIndex + 1] = this.layersData[i][sourcePixelIndex + 1];
                        data[destinationPixelIndex + 2] = this.layersData[i][sourcePixelIndex + 2];
                        data[destinationPixelIndex + 3] = this.layersData[i][sourcePixelIndex + 3];  
                    }
                }
            }

            return data;
        }

        public void CreateLayer(int width, int height, Vector2 offset)
        {
            this.currentLayer++;
            this.layersOffset.Add(offset);
            this.layersSize.Add(new Vector2(width, height));
            this.layersData.Add(new byte[width * height * 4]);
        }

        public void SetPixel(int x, int y, Color color)
        {
            var pixelIndex = y * (int)this.layersSize[this.currentLayer].X * 4 + x * 4;

            this.layersData[this.currentLayer][pixelIndex] = color.R;
            this.layersData[this.currentLayer][pixelIndex + 1] = color.G;
            this.layersData[this.currentLayer][pixelIndex + 2] = color.B;
            this.layersData[this.currentLayer][pixelIndex + 3] = color.A;
        }
    }
}
