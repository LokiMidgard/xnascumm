﻿using System;
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

        public void StartWalk(int x, int y)
        {
            // TODO : complete this method

            this.IsMoving = true;
        }

        public void Talk(string text)
        {
            talk = text;
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            costume.Draw(spriteBatch, new Vector2(position.X, position.Y - elevation), direction, this.ScaleX, this.ScaleY);
            //if (talk != null)
            //{
            //    char[] buffer = new char[talk.Length];
            //    for (int i = 0; i < talk.Length; ++i)
            //    {
            //        if ((talk[i] < 'a' || talk[i] > 'z') && (talk[i] < 'A' || talk[i] > 'Z'))
            //            buffer[i] = ' ';
            //        else
            //            buffer[i] = talk[i];
            //    }
            //    spriteBatch.DrawString(ScummEngine.font, new string(buffer), talkPosition * 2, Color.White);
            //}
        }
    }
}
