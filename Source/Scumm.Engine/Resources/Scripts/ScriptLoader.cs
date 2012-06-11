﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.Resources.Loaders;
using Scumm.Engine.IO;

namespace Scumm.Engine.Resources.Scripts
{
    class ScriptLoader : ResourceLoader
    {
        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            uint blockSize;

            if (parameters.ContainsKey("Type"))
            {
                blockSize = ScummEngine.Instance.ResourceManager.FindDataBlock((string)parameters["Type"]);
            }
            else
                blockSize = ScummEngine.Instance.ResourceManager.FindDataBlock("SCRP");

            // Read script Header information
            if (blockSize == 0)
            {
                throw new InvalidOperationException("Could not find the script header block.");
            }

            // Read the opcode blocks
            var data = reader.ReadBytes((int)blockSize - 8);
            var script = new ScriptV5(resourceId, data);
            return script;
        }
    }
}