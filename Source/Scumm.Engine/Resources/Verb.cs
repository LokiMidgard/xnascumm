using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scumm.Engine.Resources
{
    class Verb : Resource
    {
        string stream;

        public string Stream
        {
            get { return stream; }
            set { stream = value; }
        }

        public Verb(string resourceId)
            : base(resourceId)
        {
            
        }
    }
}
