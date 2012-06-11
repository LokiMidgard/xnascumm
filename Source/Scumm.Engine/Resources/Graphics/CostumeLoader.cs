using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.Resources.Loaders;
using Scumm.Engine.IO;

namespace Scumm.Engine.Resources.Graphics
{
    class CostumeLoader : ResourceLoader
    {
        public CostumeLoader()
        {
        }

        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            //uint blockSize = ScummEngine.ResourceManager.FindDataBlock("SCRP");
            //
            //// Read script Header information
            //if (blockSize == 0)
            //{
            //    throw new InvalidOperationException("Could not find the script header block.");
            //}
            //
            //// Read the opcode blocks
            //var data = reader.ReadBytes((int)blockSize - 8);
            var costume = new Costume(resourceId);
            return costume;
        }
    }
}
