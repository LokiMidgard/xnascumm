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

        public void Draw(SpriteBatch spriteBatch, int animation, int frame, Vector2 position, int direction, int scaleX, int scaleY)
        {
            if (animation != 0)
            {
                animation = animation - 4;
            }

            // TODO : Use the correct frame of the current animation
            if (this.Animations.Count >= animation + direction)
            {
                if(this.Animations[animation + direction].Frames.Count <= frame)
                {
                    //frame = this.Animations[animation + direction].Frames.Count - 1;
                }

                if (this.Animations[animation + direction].Frames[frame].Data != null)
                {
                    var currentAnimation = this.Animations[animation + direction];
                    var isMirrored = currentAnimation.IsMirrored;

                    spriteBatch.Draw(currentAnimation.Frames[frame].Data, new Rectangle((int)position.X * 2, (int)position.Y * 2 - (int)(currentAnimation.Frames[0].Data.Height * 2), (int)(currentAnimation.Frames[frame].Data.Width * 2 * ((float)scaleX / 255)), (int)(currentAnimation.Frames[frame].Data.Height * 2 * ((float)scaleY / 255))), null, Color.White, 0, Vector2.Zero, isMirrored ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                }
            }
        }
    }
}
