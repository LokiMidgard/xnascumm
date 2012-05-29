using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scumm.Engine.Resources
{
    public class Resource
    {
        string resourceId;

        protected Resource(string id)
        {
            resourceId = id;
        }

        public string ResourceId
        {
            get { return resourceId; }
        }
    }
}
