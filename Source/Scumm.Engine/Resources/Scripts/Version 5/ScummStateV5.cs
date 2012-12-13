using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Scumm.Engine.Resources.Scripts
{
    public class ScummStateV5
    {
        private Vector2 stringPos;
        private bool overhead;
        private bool centered;
        private byte currentRoomId;

        public byte CurrentRoomId
        {
            get { return currentRoomId; }
            set { currentRoomId = value; }
        }

        public bool Centered
        {
            get { return centered; }
            set { centered = value; }
        }
        public bool Overhead
        {
            get { return overhead; }
            set { overhead = value; }
        }

        public Vector2 StringPos
        {
            get { return stringPos; }
            set { stringPos = value; }
        }

        public ScummStateV5()
        {
            currentRoomId = 0;
        }

    }
}
