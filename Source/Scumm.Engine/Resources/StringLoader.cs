using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.IO;

namespace Scumm.Engine.Resources
{
    class StringLoader : ResourceLoader
    {
        public StringLoader()
        {
        }

        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            ScummString scummString = new ScummString(resourceId);

            byte b;
            b = reader.ReadByte();
            
            while (b != 0x00)
            {
                scummString.Stream = scummString.Stream + (char)b;
                b = reader.ReadByte();

                if (b == 0xFF)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        b = reader.ReadByte();
                        scummString.Stream = scummString.Stream + (char)b;
                    }
                }
            }
            
            return scummString;
        }
    }
}
