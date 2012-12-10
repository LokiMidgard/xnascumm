using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.Resources.Loaders;
using Microsoft.Xna.Framework.Graphics;
using Scumm.Engine.IO;
using Microsoft.Xna.Framework;

namespace Scumm.Engine.Resources.Graphics
{
    class ObjectImageLoader : ImageLoader
    {
        public ObjectImageLoader(GraphicsDevice device)
            : base(device)
        {
        }

        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            reader.FindDataBlockNoInfo("OBIM");
            // read header
            uint blockSize = reader.FindDataBlock("IMHD");

            UInt16 id = reader.ReadUInt16();
            var obj = ResourceManager.FindObject(id);

            UInt16 numImages = reader.ReadUInt16();
            UInt16 numZs = reader.ReadUInt16();
            Byte flags = reader.ReadByte();
            Byte unknown = reader.ReadByte();
            UInt16 x = reader.ReadUInt16();
            UInt16 y = reader.ReadUInt16();
            UInt16 width = reader.ReadUInt16();
            UInt16 height = reader.ReadUInt16();

            obj.Position = new Vector2(x, y);
            obj.Image = new Image(width, height);

            var roomPalette = (Color[])parameters["RoomPalette"];
            if (numImages > 0)
            {
                if (reader.FindDataBlock("IM01") == 0)
                {
                    throw new InvalidOperationException("Could not find image block.");
                }
                ReadImageDataBlock(reader, obj.Image, roomPalette);
            }

            return obj.Image;
        }
    }
}
