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
        public Costume(string resourceId)
            : base(resourceId)
        {
            this.Animations = new List<CostumeAnimation>();
        }

        public IList<CostumeAnimation> Animations
        {
            get;
            private set;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, int direction, int scaleX, int scaleY)
        {
            // TODO : Use the correct frame of the current animation
            if (this.Animations.Count > 0 && this.Animations[0 + direction].Frames[0].Data != null)
            {
                var currentAnimation = this.Animations[0 + direction];
                var isMirrored = currentAnimation.IsMirrored;

                spriteBatch.Draw(currentAnimation.Frames[0].Data, new Rectangle((int)position.X * 2, (int)position.Y * 2 - (int)(currentAnimation.Frames[0].Data.Height * 2), (int)(currentAnimation.Frames[0].Data.Width * 2 * ((float)scaleX / 255)), (int)(currentAnimation.Frames[0].Data.Height * 2 * ((float)scaleY / 255))), null, Color.White, 0, Vector2.Zero, isMirrored ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }
    }
}
