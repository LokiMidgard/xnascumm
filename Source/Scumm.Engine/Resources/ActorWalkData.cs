using Microsoft.Xna.Framework;
using System;

namespace Scumm.Engine.Resources
{
    public class ActorWalkData
    {
        public Vector2 CurrentPosition { get; set; }
        public Vector2 NextPosition { get; set; }
        public Vector2 Destination { get; set; }
        public int DestinationDirection { get; set; }
        public float DeltaX { get; set; }
        public float DeltaY { get; set; }
        public int XFrac { get; set; }
        public int YFrac { get; set; }
    }
}
