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
    class RoomImageLoader : ImageLoader
    {
        public RoomImageLoader(GraphicsDevice device)
            : base(device)
        {
        }

        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            var image = new Image((int)parameters["Width"], (int)parameters["Height"]);
            var roomPalette = (Color[])parameters["RoomPalette"];

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
    }
}
