using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.IO;

namespace Scumm.Engine.Resources
{
    class StringLoader : ResourceLoader
    {
        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            ScummString scummString = new ScummString();

            byte b;
            b = reader.ReadByte();
            while (b != 0x00)
            {
                scummString.Stream = scummString.Stream + (char)b;
                b = reader.ReadByte();
            }
            
            return scummString;
        }
    }
}
