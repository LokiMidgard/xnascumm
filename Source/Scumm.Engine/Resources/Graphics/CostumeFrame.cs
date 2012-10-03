using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Scumm.Engine.Resources.Graphics
{
    public class CostumeFrame
    {
        public CostumeFrame()
        {

        }

        public Texture2D Data
        {
            get;
            internal set;
        }

        public CostumeFrameType FrameType
        {
            get;
            internal set;
        }

        public Vector2 MovementVector
        {
            get;
            internal set;
        }

        public Vector2 Offset
        {
            get;
            internal set;
        }
    }
}
