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
        private EventManager eventManager;

        private Room currentRoom;
        private List<Actor> currentActors;
        private List<Object> inventory;

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

            eventManager = new EventManager();

            currentActors = new List<Actor>();
            inventory = new List<Object>();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            eventManager.Update();
            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //    this.Exit();

            //if (eventManager.Clicked(Keys.Left))
            //{
            //    if (curRoomIndex > 0)
            //    {
            //        --curRoomIndex;
            //        currentRoom = ScummEngine.Instance.ResourceManager.Load<Room>("ROOM", curRoomIndex);
            //    }
            //}
            //
            //if (eventManager.Clicked(Keys.Right))
            //{
            //    if (curRoomIndex < 82)
            //    {
            //        ++curRoomIndex;
            //        currentRoom = ScummEngine.Instance.ResourceManager.Load<Room>("ROOM", curRoomIndex);
            //    }
            //}

        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            if (currentRoom != null)
                currentRoom.Draw(SpriteBatch);

            for(int i = 0; i < currentActors.Count; ++i)
                currentActors[i].Draw(SpriteBatch);

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
