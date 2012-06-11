using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.IO;

namespace Scumm.Engine.Resources
{
    class ObjectLoader : ResourceLoader
    {
        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            ScummEngine.Instance.ResourceManager.FindDataBlock("OBCD");
            uint blockSize = ScummEngine.Instance.ResourceManager.FindDataBlock("CDHD");
            
            UInt16 id =  reader.ReadUInt16();
            var data = reader.ReadBytes((int)blockSize - 10);
            var obj = ScummEngine.Instance.ResourceManager.FindObject(id);
            return obj;
        }
    }
}
