﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.Resources.Loaders;
using Scumm.Engine.IO;
using System.IO;

namespace Scumm.Engine.Resources.Scripts
{
    class ScriptLoader : ResourceLoader
    {
        ScriptManager scriptManager;
        SceneManager sceneManager;
        StreamWriter logFile;

        public ScriptLoader(ScriptManager scriptMngr, SceneManager sceneMngr, StreamWriter logFile)
        {
            scriptManager = scriptMngr;
            sceneManager = sceneMngr;
            this.logFile = logFile;
        }

        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            uint blockSize;

            if (parameters.ContainsKey("Type"))
            {
                blockSize = reader.FindDataBlock((string)parameters["Type"]);
            }
            else
                blockSize = reader.FindDataBlock("SCRP");

            // Read script Header information
            if (blockSize == 0)
            {
                throw new InvalidOperationException("Could not find the script header block.");
            }

            // Read the opcode blocks
            var data = reader.ReadBytes((int)blockSize - 8);
            var script = new ScriptV5(resourceId, data, scriptManager, resourceManager, sceneManager, this.logFile);
            return script;
        }
    }
}
