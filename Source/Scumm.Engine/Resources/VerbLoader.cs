using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.IO;

namespace Scumm.Engine.Resources
{
    class VerbLoader : ResourceLoader
    {
        public override Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters)
        {
            Verb verb = new Verb(resourceId);

            int length = 0;
            byte b = 0xFF;
            while (b != 0x00)
            {
                b = reader.ReadByte();
                verb.Stream = verb.Stream + (char)b;
                ++length;
                if (b == 0xFF)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        byte c = reader.ReadByte();
                        verb.Stream = verb.Stream + (char)c;
                        ++length;
                    }
                }
            }

            return verb;
        }
    }
}
