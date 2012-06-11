using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.IO;

namespace Scumm.Engine.Resources
{
    public abstract class ResourceLoader
    {
        #region Fields

        protected ResourceManager resourceManager;

        #endregion

        #region Properties

        public ResourceManager ResourceManager
        {
            get { return resourceManager; }
            set { resourceManager = value; }
        }

        #endregion

        #region Methods

        public abstract Resource LoadResourceData(ScummBinaryReader reader, string resourceId, IDictionary<string, object> parameters);

        #endregion
    }
}
