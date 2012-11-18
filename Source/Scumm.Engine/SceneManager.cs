using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Scumm.Engine.Resources
{
    public class SceneManager : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private GraphicsDeviceManager graphics;

        private Room currentRoom;
        private List<Actor> currentActors;
        private List<Object> inventory;
        private List<Verb> verbs;

        public List<Verb> Verbs
        {
            get { return verbs; }
        }
        public List<Actor> CurrentActors
        {
            get { return currentActors; }
        }
        public List<Object> Inventory
        {
            get { return inventory; }
        }
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }
        public Room CurrentRoom
        {
            get { return currentRoom; }
            set { currentRoom = value; }
        }

        public SceneManager(Game game)
            : base(game)
        {
            graphics = new GraphicsDeviceManager(game);
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 400;

            currentActors = new List<Actor>();
            inventory = new List<Object>();
            verbs = new List<Verb>();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            if (currentRoom != null)
                currentRoom.Draw(SpriteBatch);

            for(int i = 0; i < currentActors.Count; ++i)
                currentActors[i].Draw(SpriteBatch);

            for (int i = 1; i < verbs.Count; ++i)
                verbs[i].Draw(SpriteBatch);

            spriteBatch.End();
        }

        public void AddObjectToInventory(Object obj)
        {
            inventory.Add(obj);
        }

        public Object GetInventoryObject(int i)
        {
            return inventory[i];
        }
    }
}
