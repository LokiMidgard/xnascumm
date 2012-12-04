using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Scumm.Engine.Resources.Scripts
{
    public class ScriptStateV5
    {
        private Vector2 stringPos;

        public Vector2 StringPos
        {
            get { return stringPos; }
            set { stringPos = value; }
        }
    }
}
