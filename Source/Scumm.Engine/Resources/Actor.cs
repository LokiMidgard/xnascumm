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
        int elevation;

        int initFrame;
        int standFrame;
        int talkStartFrame, talkStopFrame;
        int walkFrame;

        int layer;
        Vector2 position;

        byte roomId;
        int scale;

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
        public ScummString Name
        {
            get { return name; }
            set { name = value; }
        }

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
        }

        public void PutActor(int x, int y)
        {
            // TODO : Complete this method to adjust the position based on various parameters (see ScummVM source code)
            this.position = new Vector2(x, y);
        }

        public void Talk(string text)
        {
            talk = text;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            costume.Draw(spriteBatch, position);
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
