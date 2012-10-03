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

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            // TODO : Use the correct frame of the current animation
            if (this.Animations.Count > 0 && this.Animations[0].Frames[0].Data != null)
            {
                var currentAnimation = this.Animations[0];
                var isMirrored = currentAnimation.IsMirrored;

                // TO REMOVE: Hardcode the position for now because the script engine is not yet complete for actor positionning
                position = new Vector2(280, 180);

                spriteBatch.Draw(currentAnimation.Frames[0].Data, new Rectangle((int)position.X, (int)position.Y, currentAnimation.Frames[0].Data.Width * 2, currentAnimation.Frames[0].Data.Height * 2), null, Color.White, 0, Vector2.Zero, isMirrored ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }
    }
}
