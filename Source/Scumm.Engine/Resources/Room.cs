using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Scumm.Engine.Resources
{
    public class Room : Resource
    {
        Image backgroundImage;
        Color[] palette;

        public Image BackgroundImage
        {
            get { return backgroundImage; }
            set { backgroundImage = value; }
        }
        
        public Color[] Palette
        {
            get { return palette; }
            set { palette = value; }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            backgroundImage.Draw(spriteBatch);
        }
    }
}
