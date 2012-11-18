using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Scumm.Engine.Resources.Scripts;

namespace Scumm.Engine.Resources
{
    public class Room : Resource
    {
        Image backgroundImage;
        Color[] palette;

        Object[] objects;
        Script entryScript, exitScript;
        Script[] scripts;

        public Room(string resourceId)
            : base(resourceId)
        {
        }

        public Script ExitScript
        {
            get { return exitScript; }
            set { exitScript = value; }
        }

        public Script EntryScript
        {
            get { return entryScript; }
            set { entryScript = value; }
        }
        public Script[] Scripts
        {
            get { return scripts; }
            set { scripts = value; }
        }

        public Image BackgroundImage
        {
            get { return backgroundImage; }
            set { backgroundImage = value; }
        }

        public Object[] Objects
        {
            get { return objects; }
            set { objects = value; }
        }

        public Color[] Palette
        {
            get { return palette; }
            set { palette = value; }
        }

        public Object getObject(UInt16 objectId)
        {
            for(int i = 0; i < Objects.Length; ++i)
                if(objects[i].Id == objectId)
                    return objects[i];
            return null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            backgroundImage.Draw(spriteBatch);
        }
    }
}
