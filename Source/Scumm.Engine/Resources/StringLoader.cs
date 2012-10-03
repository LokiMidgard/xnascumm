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

            var data = new List<byte>();

            byte b;
            b = reader.ReadByte();
            
            while (b != 0x00)
            {
                data.Add(b);
                b = reader.ReadByte();

                if (b == 0xFF)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        b = reader.ReadByte();
                        data.Add(b);
                    }
                }
            }
            scummString.Stream = new string(System.Text.Encoding.GetEncoding("IBM437").GetChars(data.ToArray()));
            return scummString;
        }
    }
}
