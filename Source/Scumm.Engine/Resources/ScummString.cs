using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scumm.Engine.Resources
{
    public class ScummString : Resource
    {
        string stream;

        public string Stream
        {
            get { return stream; }
            set { stream = value; }
        }

        public ScummString(string resourceId)
            : base(resourceId)
        {
            
        }
    }
}
