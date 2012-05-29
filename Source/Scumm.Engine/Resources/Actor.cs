using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.Resources.Graphics;
using Microsoft.Xna.Framework;

namespace Scumm.Engine.Resources
{
    class Actor
    {
        public Costume Costume
        {
            get;
            set;
        }

        public int CostumeId
        {
            get;
            set;
        }

        public int Elevation
        {
            get;
            set;
        }

        public int InitFrame
        {
            get;
            set;
        }

        public int Layer
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public Vector2 Position
        {
            get;
            set;
        }

        public int RoomId
        {
            get;
            set;
        }

        public int Scale
        {
            get;
            set;
        }

        // TODO: Convert that to an enum?
        public int ShadowMode
        {
            get;
            set;
        }

        public int StandFrame
        {
            get;
            set;
        }

        public int TalkColor
        {
            get;
            set;
        }

        public Vector2 TalkPosition
        {
            get;
            set;
        }

        public int TalkStartFrame
        {
            get;
            set;
        }

        public int TalkStopFrame
        {
            get;
            set;
        }

        public int WalkFrame
        {
            get;
            set;
        }

        public int Width
        {
            get;
            set;
        }

        public void Init(int mode)
        {
            // TODO : Check mode -1 and 1 ???
            if (mode == 2)
            {
                //_facing = 180;
            }

            this.Elevation = 0;

            // TODO: Remove that
            this.Elevation = -40;

            this.Width = 24;
            this.TalkColor = 15;
            this.TalkPosition = new Vector2(0, -80);

            // TODO: Review scale calculation
            this.Scale = 1;
            this.Layer = 0;

            //_charset = 0;
            //_targetFacing = _facing;

            this.ShadowMode = 0;

            /// TODO: Implement that code!
            //stopActorMoving();
            //setActorWalkSpeed(8, 2);

            //_animSpeed = 0;
            //_animProgress = 0;

            //_ignoreBoxes = false;
            //_forceClip = 0;
            //_ignoreTurns = false;

            //_talkFrequency = 256;
            //_talkPan = 64;
            //_talkVolume = 127;

            this.InitFrame = 1;
            this.WalkFrame = 2;
            this.StandFrame = 3;
            this.TalkStartFrame = 4;
            this.TalkStopFrame = 5;

            //_walkScript = 0;
            //_talkScript = 0;
        }
    }
}
