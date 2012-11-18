using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.IO;
using Scumm.Engine.Resources.Scripts;

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
            
            // read header
            uint blockSize = reader.FindDataBlock("CDHD");
            UInt16 id = reader.ReadUInt16();
            var data = reader.ReadBytes((int)blockSize - 10);

            var obj = ResourceManager.FindObject(id);
            if (id == 488)
            {
                // read verb block
                blockSize = reader.FindDataBlock("VERB");
                long verbPos = reader.BaseStream.Position - 8;

                // read verbs and offsets
                byte[] verbs = new byte[100];
                UInt16[] offsets = new UInt16[100];

                int totalVerbs = 0;
                verbs[totalVerbs] = reader.ReadByte();
                while (verbs[totalVerbs] != 0)
                {
                    offsets[totalVerbs] = reader.ReadUInt16();
                    verbs[++totalVerbs] = reader.ReadByte();
                }
                // final offset found reading the next block - needed for blocksize
                blockSize = reader.FindDataBlockNoInfo("OBNA");
                long endPos = reader.BaseStream.Position - 8;
                
                // read object name
                byte a = reader.ReadByte();
                while (a != 0)
                {
                    a = reader.ReadByte();
                    obj.Name += (char)a;
                }
                long backupPos = reader.BaseStream.Position;

                // load verb scripts
                for (int i = 0; i < totalVerbs; ++i)
                {
                    long startPos = verbPos + offsets[i];
                    uint size = (uint)(endPos - startPos);

                    obj.VerbScript[verbs[i]] = 
                        (ScriptV5)resourceManager.Load<Script>("SCRP", id, reader, new Dictionary<string, object>() { { "Position", startPos }, { "Blocksize", size } });
                }
                reader.BaseStream.Position = backupPos;
            }
            return obj;
        }
    }
}
