using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.Resources.Graphics;
using Microsoft.Xna.Framework;

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

        #region Properties

        public byte RoomID
        {
            get { return roomId; }
            set { roomId = value; }
        }

        #endregion

        public void Init(int mode)
        {
            if (mode != 0)
            {
                roomId = 0;
                position = Vector2.Zero;
            }

            this.elevation = 0;
            this.width = 24;
            this.talkColor = 15;

            this.initFrame = 1;
            this.walkFrame = 2;
            this.standFrame = 3;
            this.talkStartFrame = 4;
            this.talkStopFrame = 5;
        }
    }
}
