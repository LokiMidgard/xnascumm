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
        public RoomLoader()
        {
        }

        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            var room = new Room(resourceId);
            var roomOffset = (uint)reader.BaseStream.Position;

            // Read Room Header information
            if (reader.FindDataBlock("RMHD", roomOffset) == 0)
            {
                throw new InvalidOperationException("Could not find the room header block.");
            }

            var width = reader.ReadUInt16();
            var height = reader.ReadUInt16();
            var objectsCount = reader.ReadUInt16();

            // Read palette data
            if (reader.FindDataBlock("CLUT", roomOffset) == 0)
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

            // Read background image
            room.BackgroundImage = ResourceManager.Load<Image>("RMIM", new Dictionary<string, object>() { { "Width", (int)width }, { "Height", (int)height }, { "RoomOffset", roomOffset }, { "RoomPalette", room.Palette } });

            room.Objects = new Object[objectsCount];

            Image[] images = null;
            if(resourceId == "ROOM_10")
            {
                images = new Image[objectsCount];
                for (int i = 0; i < objectsCount; ++i)
                {
                    images[i] = ResourceManager.Load<Image>("OBIM", new Dictionary<string, object>() { { "RoomPalette", room.Palette } });
                }
            }
            for (int i = 0; i < objectsCount; ++i)
            {
                Object obj = ResourceManager.Load<Object>("OBJC", new Dictionary<string, object>());
                room.Objects[i] = obj;
                if (resourceId == "ROOM_10") room.Objects[i].Image = images[i];
            }

            // Read entry/exit scripts
            room.ExitScript = ResourceManager.Load<Script>("SCRP", new Dictionary<string, object>() { { "Type", "EXCD" } });
            room.EntryScript = ResourceManager.Load<Script>("SCRP", new Dictionary<string, object>() { { "Type", "ENCD" } });

            // Read local script
            if (reader.FindDataBlock("NLSC") != 0)
            {
                byte totalLocal = reader.ReadByte();
                byte padding = reader.ReadByte();
                if (totalLocal != 0)
                {
                    room.Scripts = new Script[totalLocal];
                    for (int i = 0; i < totalLocal; ++i)
                    {
                        room.Scripts[i] = resourceManager.Load<Script>("SCRP", new Dictionary<string, object>() { { "Type", "LSCR" } });
                    }
                }
            }

            return room;
        }
    }
}
