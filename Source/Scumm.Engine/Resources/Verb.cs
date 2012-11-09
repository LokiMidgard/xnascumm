using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Scumm.Engine.Resources
{
    public class Verb : Resource
    {
        string stream = "";
        Vector2 position = Vector2.Zero;

        public string Stream
        {
            get { return stream; }
            set { stream = value; }
        }

        public int X
        {
            set { position.X = value; }
        }

        public int Y
        {
            set { position.Y = value; }
        }

        public Verb(string resourceId)
            : base(resourceId)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            char[] buffer = new char[stream.Length];
            for (int i = 0; i < stream.Length; ++i) 
            {
                if ((stream[i] < 'a' || stream[i] > 'z') && (stream[i] < 'A' || stream[i] > 'Z'))
                    buffer[i] = ' ';
                else
                    buffer[i] = stream[i];
            }

            spriteBatch.DrawString(ScummEngine.font, new string(buffer), position * 2, Color.White);
        }
    }
}
