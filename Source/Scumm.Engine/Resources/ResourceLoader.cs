using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.IO;

namespace Scumm.Engine.Resources
{
    public abstract class ResourceLoader
    {
        public abstract Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters);
    }
}
