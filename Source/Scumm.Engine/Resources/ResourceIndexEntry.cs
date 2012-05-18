using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scumm.Engine.Resources
{
    public class ResourceIndexEntry
    {
        public ResourceIndexEntry(byte roomId, uint offset)
        {
            this.RoomId = roomId;
            this.Offset = offset;
        }

        public byte RoomId
        {
            get;
            private set;
        }

        public uint Offset
        {
            get;
            private set;
        }
    }
}
