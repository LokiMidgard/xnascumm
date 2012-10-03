using System;
using System.Collections.Generic;

namespace Scumm.Engine.Resources.Graphics
{
    public class CostumeAnimation
    {
        public CostumeAnimation()
        {
            this.Frames = new List<CostumeFrame>();
        }

        public IList<CostumeFrame> Frames
        {
            get;
            private set;
        }

        public bool IsLooped
        {
            get;
            set;
        }

        public bool IsMirrored
        {
            get;
            set;
        }
    }
}
