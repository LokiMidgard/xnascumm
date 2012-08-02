using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Scumm.Engine.Resources.Graphics
{
    public class Costume : Resource
    {
        public Texture2D TestTexture
        {
            get;
            internal set;
        }

        public Costume(string resourceId)
            : base(resourceId)
        {
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (TestTexture != null)
                spriteBatch.Draw(TestTexture, new Rectangle((int)position.X, (int)position.Y, TestTexture.Width * 2, TestTexture.Height * 2), Color.White);
        }
    }
}
