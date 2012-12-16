using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.Resources.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scumm.Engine.Resources.Scripts;

namespace Scumm.Engine.Resources
{
    [Flags]
    public enum MoveFlags
    {
        NewLeg = 1,
        InLeg = 2,
        Turn = 4,
        LastLeg = 8,
        Frozen = 0x80
    };


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
        int facingAngle;
        int targetFacingAngle;
        bool ignoreTurns;

        int layer;
        Vector2 position;

        byte roomId;

        string talk;
        int talkColor;
        Vector2 talkPosition;
        Charset charset;
        int width;

        private int currentAnimation;
        private int currentAnimationProgress;

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
        public MoveFlags moving { get; set; }
        public bool IsWalking { get; set; }
        public bool IgnoreBoxes { get; set; }
        public int WalkSpeedX { get; set; }
        public int WalkSpeedY { get; set; }
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

            this.WalkSpeedX = 8;
            this.WalkSpeedY = 2;

            this.targetFacingAngle = this.facingAngle;

            this.currentAnimation = initFrame;
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

        private void StartAnimation(int animation)
        {
            //if (isInCurrentRoom() && _costume != 0)
            //{
                this.currentAnimationProgress = 0;

                //if (_vm->_game.version >= 3 && f == _initFrame)
                //{
                //    _cost.reset();

                //}

                this.currentAnimation = animation;
            //}
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
                //if (dir != _facing)
                //    turnToDirection(dir);
                //return;
            //}

            this.walkData = new ActorWalkData();
            this.walkData.Destination = new Vector2(x, y);
            //_walkdata.destbox = abr.box;
            this.walkData.DestinationDirection = direction;
            this.moving = (this.moving & MoveFlags.InLeg) | MoveFlags.NewLeg;

            //_walkdata.point3.x = 32000;

            //_walkdata.curbox = _walkbox;

            // TODO : complete this method
            this.IsWalking = true;
        }

        private void WalkActor()
        {
            if (this.moving == 0)
            {
                return;
            }

            if ((this.moving & MoveFlags.NewLeg) == 0)
            {
                if ((this.moving & MoveFlags.InLeg) != 0 && WalkStep() != 0)
                {
                    return;
                }

                if ((this.moving & MoveFlags.LastLeg) != 0)
                {
                    this.moving = 0;
                    //setBox(_walkdata.destbox);
                    
                    StartAnimation(this.standFrame);

                    if (this.targetFacingAngle != this.walkData.DestinationDirection)
                    {
                        TurnToDirection(this.walkData.DestinationDirection);
                    }

                    return;
                }

                if ((this.moving & MoveFlags.Turn) != 0)
                {
                    //var new_dir = UpdateActorDirection(false);

                    //if (facingAngle != new_dir)
                    //{
                    //    this.facingAngle = this.NormalizeAngle(new_dir);
                    //    this.direction = this.AngleToSimpleDirection(facingAngle);
                    //}

                    //else
                    //{
                    //    this.moving = 0;
                    //}

                    return;
                }

                //setBox(_walkdata.curbox);
                this.moving &= MoveFlags.InLeg;
            }

            this.moving &= ~MoveFlags.NewLeg;

            //do
            //{
                //if (_walkbox == kInvalidBox)
                //{
                //    setBox(_walkdata.destbox);
                //    _walkdata.curbox = _walkdata.destbox;
                //    break;
                //}

                //if (_walkbox == _walkdata.destbox)
                //    break;

                //next_box = _vm->getNextBox(_walkbox, _walkdata.destbox);
                //if (next_box < 0)
                //{
                //    _walkdata.destbox = _walkbox;
                //    _moving |= MF_LAST_LEG;
                //    return;
                //}

                //_walkdata.curbox = next_box;

                //if (findPathTowards(_walkbox, next_box, _walkdata.destbox, foundPath))
                //    break;

                //if (calcMovementFactor(foundPath))
                //    return;

                //setBox(_walkdata.curbox);

            //} while (true);

            this.moving |= MoveFlags.LastLeg;
            CalcMovementFactor(this.walkData.Destination);
        }

        private int CalcMovementFactor(Vector2 next) 
        {
            if (this.position == next)
            {
                return 0;
            }

	        var diffX = (int)(next.X - this.position.X);
	        var diffY = (int)(next.Y - this.position.Y);

	        float deltaY = this.WalkSpeedY;

            if (diffY < 0)
            {
                deltaY = -deltaY;
            }

	        float deltaX = deltaY * diffX;

	        if (diffY != 0) 
            {
		        deltaX /= diffY;
	        } 
            else 
            {
		        deltaY = 0;
	        }

            if (Math.Abs(deltaX) > this.WalkSpeedX)	
            {
                deltaX = this.WalkSpeedX;

                if (diffX < 0)
                {
                    deltaX = -deltaX;
                }

		        deltaY = deltaX * diffY;

		        if (diffX != 0) 
                {
			        deltaY /= diffX;
		        } 

                else 
                {
			        deltaX = 0;
		        }
	        }

	        this.walkData.CurrentPosition = this.position;
            this.walkData.NextPosition = next;
            this.walkData.DeltaX = deltaX;
            this.walkData.DeltaY = deltaY;

            this.targetFacingAngle = GetAngleFromPos(deltaX, deltaY);

	        return this.WalkStep();
        }

        private int GetAngleFromPos(float x, float y)
        {
            if (Math.Abs(y) * 2 < Math.Abs(x))
            {
                if (x > 0)
                    return 90;

                return 270;
            }
            else
            {
                if (y > 0)
                    return 180;

                return 0;
            }
        }

        private void TurnToDirection(int newdir) 
        {
            if (newdir == -1 || this.ignoreTurns)
		        return;

		    this.moving = MoveFlags.Turn;
		    this.targetFacingAngle = newdir;
            this.direction = AngleToSimpleDirection(newdir);
        }

        private int WalkStep()
        {
            var nextFacingAngle = ComputeActorAngle(true);

            if ((this.moving & MoveFlags.InLeg) == 0 || this.facingAngle != nextFacingAngle)
            {
                if (this.currentAnimation != this.walkFrame || this.facingAngle != nextFacingAngle)
                {
                    StartWalkAnimation(1, nextFacingAngle);
                }

                this.moving |= MoveFlags.InLeg;
            }

            //if (_walkbox != _walkdata.curbox && _vm->checkXYInBoxBounds(_walkdata.curbox, _pos.x, _pos.y))
            //{
            //    setBox(_walkdata.curbox);
            //}

            var distanceX = (int)Math.Abs(this.walkData.NextPosition.X - this.walkData.CurrentPosition.X);
            var distanceY = (int)Math.Abs(this.walkData.NextPosition.Y - this.walkData.CurrentPosition.Y);

            if (Math.Abs(this.position.X - this.walkData.CurrentPosition.X) >= distanceX && Math.Abs(this.position.Y - this.walkData.CurrentPosition.Y) >= distanceY)
            {
                this.moving &= ~MoveFlags.InLeg;
                return 0;
            }

            var positionX = this.position.X + this.walkData.DeltaX * ((float)this.ScaleX / 255);
            var positionY = this.position.Y + this.walkData.DeltaY * ((float)this.ScaleY / 255);

            if (Math.Abs(positionX - this.walkData.CurrentPosition.X) > distanceX)
            {
                positionX = (int)this.walkData.NextPosition.X;
            }

            if (Math.Abs(positionY - this.walkData.CurrentPosition.Y) > distanceY)
            {
                positionY = (int)this.walkData.NextPosition.Y;
            }

            this.position = new Vector2(positionX, positionY);

            if (this.position == this.walkData.NextPosition)
            {
                this.moving &= ~MoveFlags.InLeg;
                this.IsWalking = false;
                return 0;
            }

            return 1;
        }

        private void StartWalkAnimation(int cmd, int angle) 
        {
	        if (angle == -1)
            {
		        angle = this.facingAngle;
            }

            //if (_walkScript) {
            //    int args[16];
            //    memset(args, 0, sizeof(args));
            //    args[0] = _number;
            //    args[1] = cmd;
            //    args[2] = angle;
            //    _vm->runScript(_walkScript, 1, 0, args);
            //} else {
		        switch (cmd) 
                {
		            case 1:										/* start walk */
                        this.facingAngle = this.NormalizeAngle(angle);
                        this.direction = this.AngleToSimpleDirection(this.facingAngle);
			            StartAnimation(this.walkFrame);
			            break;
		            case 2:										/* change dir only */
                        this.facingAngle = this.NormalizeAngle(angle);
                        this.direction = this.AngleToSimpleDirection(this.facingAngle);
			            break;
		            case 3:										/* stop walk */
			            //turnToDirection(angle);
                        StartAnimation(this.standFrame);
			            break;
		        }
	        //}
        }

        private int ComputeActorAngle(bool isWalking) 
        {
	        int dir;
	        bool shouldInterpolate;

            dir = RemapDirection(this.targetFacingAngle, isWalking);

		    shouldInterpolate = (dir & 1024) != 0 ? true : false;
	        //dir &= 1023;

	        if (shouldInterpolate) 
            {
                var fromDirection = this.facingAngle;

                if (fromDirection < this.targetFacingAngle)
                {
                    fromDirection += 360;
                }

                var deltaAngle = fromDirection - this.targetFacingAngle;

                if (deltaAngle > 0)
                {
                    dir = (this.facingAngle - (deltaAngle / (deltaAngle / 90))) % 360;
                }

                else if (deltaAngle < 0)
                {
                    dir = (this.facingAngle + (deltaAngle / (deltaAngle / 90))) % 360;
                }

                else
                {
                    dir = this.targetFacingAngle;
                }
	        }

	        return dir;
        }
        
        private int RemapDirection(int dir, bool isWalking) 
        {
	        int specdir;
	        byte flags;
	        byte mask;
	        bool flipX;
	        bool flipY;
	        
	        if (!this.IgnoreBoxes) 
            {
                //specdir = _vm->_extraBoxFlags[_walkbox];

                //if (specdir) {
                //    if (specdir & 0x8000) {
                //        dir = specdir & 0x3FFF;
                //    } else {
                //        specdir = specdir & 0x3FFF;
                //        if (specdir - 90 < dir && dir < specdir + 90)
                //            dir = specdir;
                //        else
                //            dir = specdir + 180;
                //    }
                //}

		        //flags = _vm->getBoxFlags(_walkbox);
                flags = 0;

		        flipX = (this.walkData.DeltaX > 0);
                flipY = (this.walkData.DeltaY > 0);

		        // Check for X-Flip
                //if ((flags & kBoxXFlip) || isInClass(kObjectClassXFlip))
                //{
                //    dir = 360 - dir;
                //    flipX = !flipX;
                //}
                //// Check for Y-Flip
                //if ((flags & kBoxYFlip) || isInClass(kObjectClassYFlip))
                //{
                //    dir = 180 - dir;
                //    flipY = !flipY;
                //}

		        switch (flags & 7) {
		        case 1:
                    if (this.IsWalking)	                       // Actor is walking
					    return flipX ? 90 : 270;
				    else	                               // Actor is standing/turning
					    return (dir == 90) ? 90 : 270;
		        case 2:
				    if (this.IsWalking)	                       // Actor is walking
					    return flipY ? 180 : 0;
				    else	                               // Actor is standing/turning
					    return (dir == 0) ? 0 : 180;
		        case 3:
			        return 270;
		        case 4:
			        return 90;
		        case 5:
			        return 0;
		        case 6:
			        return 180;
		        }
	        }

	        // OR 1024 in to signal direction interpolation should be done
	        return NormalizeAngle(dir) | 1024;
        }

        /**
             * Normalize the given angle - that means, ensure it is positive, and
             * change it to the closest multiple of 45 degree by abusing toSimpleDir.
             */
        int NormalizeAngle(int angle)
        {
            int temp;

            temp = (angle + 360) % 360;

            return temp;// AngleToSimpleDirection(temp) * 45;
        }


        private int AngleToSimpleDirection(int angle) 
        {
            if (angle >= 270)
            {
                return 0;
            }

            else if (angle >= 180)
            {
                return 3;
            }

            else if (angle >= 90)
            {
                return 1;
            }

            return 2;
        }

        private int SimpleDirectionToAngle(int dirType, int dir)
        {
            return dir * 90;
        }

        public void Talk(string text, Charset textCharset, ScummStateV5 state)
        {
            talk = text;
            charset = textCharset;

            // draw overhead
            if (state.Overhead)
            {
                talkPosition.X = position.X;
                if (state.Centered)
                {
                    talkPosition.X -= charset.GetTextWidth(talk)/2;
                }
            }

        }

        public void Update()
        {
            WalkActor();

            // HACK: To refactor
            if (this.costume != null && this.costume.Animations.Count > (this.currentAnimation * 4 - 4) && this.costume.Animations[this.currentAnimation * 4 - 4].Frames.Count <= this.currentAnimationProgress + 1)
            {
                this.currentAnimationProgress = 0;
            }

            else
            {
                this.currentAnimationProgress++;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (costume != null)
                costume.Draw(spriteBatch, this.currentAnimation * 4, this.currentAnimationProgress, new Vector2(position.X, position.Y - elevation), direction, this.ScaleX, this.ScaleY);

            if (talk != null && charset != null)
                charset.DrawText(spriteBatch, talk, talkPosition);
        }
    }
}
