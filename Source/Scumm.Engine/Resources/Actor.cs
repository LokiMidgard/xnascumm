using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.Resources.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scumm.Engine.Resources
{
    public class Actor
    {
        Costume costume;
        ActorWalkData walkData;
        int elevation;

        int initFrame;
        int standFrame;
        int talkStartFrame, talkStopFrame;
        int walkFrame;
        int direction;

        int layer;
        Vector2 position;

        byte roomId;

        string talk;
        int talkColor;
        Vector2 talkPosition;
        Charset charset;
        int width;

        ScummString name;

        #region Properties

        public byte RoomID
        {
            get { return roomId; }
            set { roomId = value; }
        }
        public Costume Costume
        {
            get { return costume; }
            set { costume = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 TalkPosition
        {
            get { return talkPosition; }
            set { talkPosition = value; }
        }
        public int Elevation
        {
            get { return elevation; }
            set { elevation = value; }
        }
        public ScummString Name
        {
            get { return name; }
            set { name = value; }
        }
        public int ScaleX { get; set; }
        public int ScaleY { get; set; }
        public bool IsMoving { get; set; }
        #endregion

        public void Init(int mode)
        {
            if (mode != 0)
            {
                roomId = 0;
                position = Vector2.Zero;
            }

            this.costume = new Costume("");
            this.elevation = 0;
            this.width = 24;
            this.talkColor = 15;
            this.talkPosition = new Vector2(50, 50);

            this.initFrame = 1;
            this.walkFrame = 2;
            this.standFrame = 3;
            this.talkStartFrame = 4;
            this.talkStopFrame = 5;

            this.ScaleX = 255;
            this.ScaleY = 255;
        }

        public void Animate(int animation)
        {
            var command = animation / 4;
            var direction = animation % 4;

            // Convert into old cmd code
            command = 0x3F - command + 2;

            switch (command)
            {
                case 2:				// stop walking
                    //startAnimActor(_standFrame);
                    //stopActorMoving();
                    Console.WriteLine("Actor => Stop Talking");
                    break;

                case 3:				// change direction immediatly
                    //_moving &= ~MF_TURN;
                    //setDirection(dir);
                    this.direction = direction;

                    Console.WriteLine("Actor => Change direction immediatly");
                    break;

                case 4:				// turn to new direction
                    //turnToDirection(dir);
                    Console.WriteLine("Actor => Turn to new direction");
                    break;

                default:
                    //startAnimActor(anim);
                    Console.WriteLine("Actor => Start animation");
                    break;
            }
        }

        public void PutActor(int x, int y)
        {
            // TODO : Complete this method to adjust the position based on various parameters (see ScummVM source code)
            this.position = new Vector2(x, y);
        }

        public void StartWalk(int x, int y, int direction)
        {
            //AdjustBoxResult abr;
            //abr = adjustXYToBeInBox(destX, destY);

            //if (!isInCurrentRoom())
            //{
            //    _pos.x = abr.x;
            //    _pos.y = abr.y;
            //    if (!_ignoreTurns && dir != -1)
            //        _facing = dir;
            //    return;
            //}

           
            //if (_ignoreBoxes)
            //{
            //    abr.box = kInvalidBox;
            //    _walkbox = kInvalidBox;
            //}
            //else
            //{
            //    if (_vm->checkXYInBoxBounds(_walkdata.destbox, abr.x, abr.y))
            //    {
            //        abr.box = _walkdata.destbox;
            //    }
            //    else
            //    {
            //        abr = adjustXYToBeInBox(abr.x, abr.y);
            //    }
            //    if (_moving && _walkdata.destdir == dir && _walkdata.dest.x == abr.x && _walkdata.dest.y == abr.y)
            //        return;
            //}

            //if (_pos.x == abr.x && _pos.y == abr.y)
            //{
            //    if (dir != _facing)
            //        turnToDirection(dir);
            //    return;
            //}

            this.walkData = new ActorWalkData();
            walkData.Destination = new Vector2(x, y);
            //_walkdata.destbox = abr.box;
            walkData.DestinationDirection = direction;
            //_moving = (_moving & MF_IN_LEG) | MF_NEW_LEG;
            //_walkdata.point3.x = 32000;

            //_walkdata.curbox = _walkbox;

            // TODO : complete this method
            this.IsMoving = true;
        }

        private void WalkStep()
        {
            int x = (int)position.X;
            int y = (int)position.Y; ;

            if (position == this.walkData.Destination)
            {
                this.IsMoving = false;
                return;
            }

            if (position.X != this.walkData.Destination.X)
            {
                x = (int)position.X + (int)Math.Sign(this.walkData.Destination.X - position.X);
            }

            if (position.Y != this.walkData.Destination.Y)
            {
                y = (int)position.Y + (int)Math.Sign(this.walkData.Destination.Y - position.Y);
            }

            this.position = new Vector2(x, y);
        }

        public void Talk(string text, Charset textCharset)
        {
            talk = text;
            charset = textCharset;
        }

        public void Update()
        {
            if (this.IsMoving)
            {
                WalkStep();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (costume != null)
                costume.Draw(spriteBatch, new Vector2(position.X, position.Y - elevation), direction, this.ScaleX, this.ScaleY);

            if (talk != null && charset != null)
                charset.DrawText(spriteBatch, talk, talkPosition*2);
        }
    }
}
