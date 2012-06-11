using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.IO;

namespace Scumm.Engine.Resources
{
    class ObjectLoader : ResourceLoader
    {
        public ObjectLoader()
        {
        }

        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            reader.FindDataBlock("OBCD");
            uint blockSize = reader.FindDataBlock("CDHD");
            
            UInt16 id =  reader.ReadUInt16();
            var data = reader.ReadBytes((int)blockSize - 10);
            var obj = ResourceManager.FindObject(id);
            return obj;
        }
    }
}
