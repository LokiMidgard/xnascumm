using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Scumm.Engine.Resources.Graphics;

namespace Scumm.Engine.Resources
{
    public class Verb : Resource
    {
        public enum Type
        {
            StringVerb,
            BitmapVerb
        };

        Type type;
        Vector2 position = Vector2.Zero;

        // String verbs
        Charset charset;
        string stream;

        // Bitmap verbs
        Image image;
        
        public Type VerbType
        {
            get { return type; }
            set { type = value; }
        }
        public Image Image
        {
            get { return image; }
            set { image = value; }
        }
        public string Stream
        {
            get { return stream; }
            set { stream = value; }
        }
        public Charset Charset
        {
            get { return charset; }
            set { charset = value; }
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
            stream = "";
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (type == Type.StringVerb)
            {
                if (charset != null)
                    charset.DrawText(spriteBatch, stream, position);
            }
            else
            {
                if(image != null)
                    image.Draw(spriteBatch, position);
            }
        }
    }
}
