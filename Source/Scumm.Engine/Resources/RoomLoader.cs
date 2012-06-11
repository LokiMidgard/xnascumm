using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.IO;
using Microsoft.Xna.Framework;
using Scumm.Engine.Resources.Scripts;

namespace Scumm.Engine.Resources.Loaders
{
    public class RoomLoader : ResourceLoader
    {
        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            Console.Write("Room Loader", "Loading Room {0}", resourceId);

            var room = new Room(resourceId);
            var roomOffset = (uint)reader.BaseStream.Position;

            // Read Room Header information
            if (ScummEngine.Instance.ResourceManager.FindDataBlock("RMHD", roomOffset) == 0)
            {
                throw new InvalidOperationException("Could not find the room header block.");
            }

            var width = reader.ReadUInt16();
            var height = reader.ReadUInt16();
            var objectsCount = reader.ReadUInt16();

            Console.Write("Room Loader", "Width: {0}, Height: {1} (Objects Count: {2})", width, height, objectsCount);

            // Read palette data
            if (ScummEngine.Instance.ResourceManager.FindDataBlock("CLUT", roomOffset) == 0)
            {
                throw new InvalidOperationException("Could not find the room palette block.");
            }

            // Load only the first palette for now
            room.Palette = new Color[256];
            for (int i = 0; i < 256; ++i)
            {
                room.Palette[i].R = reader.ReadByte();
                room.Palette[i].G = reader.ReadByte();
                room.Palette[i].B = reader.ReadByte();
            }

            room.Objects = new Object[objectsCount];
            for (int i = 0; i < objectsCount; ++i)
            {
                Object obj = ScummEngine.Instance.ResourceManager.Load<Object>("OBJC", new Dictionary<string, object>());
                room.Objects[i] = obj;
            }

            // Read entry/exit scripts
            room.ExitScript = ScummEngine.Instance.ResourceManager.Load<Script>("SCRP", new Dictionary<string, object>() { { "Type", "EXCD" } });
            room.EntryScript = ScummEngine.Instance.ResourceManager.Load<Script>("SCRP", new Dictionary<string, object>() { { "Type", "ENCD" } });

            // Read background image
            room.BackgroundImage = ScummEngine.Instance.ResourceManager.Load<Image>("RMIM", new Dictionary<string, object>() { { "Width", (int)width }, { "Height", (int)height }, { "RoomOffset", roomOffset }, { "RoomPalette", room.Palette } });

            return room;
        }
    }
}
