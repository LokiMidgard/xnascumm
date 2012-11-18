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

        public int layer;
        string name;
        Vector2 position;

        byte roomId;
        int scale;
  
        int talkColor;
        Vector2 talkPosition;
        int width;

        Byte ownerState;
        UInt32 classData;

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

        public UInt32 ClassData
        {
            get { return classData; }
            set { classData = value; }
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

        public void Talk()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            costume.Draw(spriteBatch, position);
        }
    }
}
