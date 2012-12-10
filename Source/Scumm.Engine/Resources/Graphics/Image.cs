using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Scumm.Engine.Resources
{
    public class Image : Resource
    {
        public Image(int width, int height)
            : base("")
        {
            this.Width = width;
            this.Height = height;
        }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public Texture2D Texture
        {
            get;
            internal set;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if(Texture != null)
                spriteBatch.Draw(Texture, new Rectangle((int)position.X * 2, (int)position.Y * 2, Width * 2, Height * 2), Color.White);
        }
    }
}
