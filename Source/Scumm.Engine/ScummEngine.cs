using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Scumm.Engine.Resources;
using Scumm.Engine.Resources.Scripts;
using System.IO;
using Scumm.Engine.Resources.Loaders;
using Scumm.Engine.Resources.Graphics;

namespace Scumm.Engine
{
    public class ScummEngine : Game
    {
        // for debugging purposes
        public static SpriteFont font;

        private static ScummEngine instance;      
        private SceneManager sceneManager;
        private ResourceManager resourceManager;
        private ScriptManager scriptManager;
        private StreamWriter logFile;
        
        public StreamWriter LogFile
        {
            get { return logFile; }
            set { logFile = value; }
        }
        public ResourceManager ResourceManager
        {
            get { return resourceManager; }
            private set { resourceManager = value; }
        }
        public ScriptManager ScriptManager
        {
            get { return scriptManager; }
            private set { scriptManager = value; }
        }
        public SceneManager SceneManager
        {
            get { return sceneManager; }
            private set { sceneManager = value; }
        }
                
        public ScummEngine(string gamePath, string gameId, int scummVersion)
        {
            // Log file
            logFile = new StreamWriter("Scumm.log");
            logFile.AutoFlush = true;

            // Initialize scene manager
            SceneManager = new SceneManager(this);
            Components.Add(SceneManager);

            // Initialize resource manager
            ResourceManager = new ResourceManager(gamePath, gameId, 5);

            // Initialize script manager
            ScriptManager = new ScriptManager(this.ResourceManager);
        }

        protected override void Initialize()
        {
            base.Initialize();

        }

        protected override void LoadContent()
        {
            base.LoadContent();
            
            // Add loaders
            ResourceManager.AddLoader("ROOM", new RoomLoader());
            ResourceManager.AddLoader("RMIM", new ImageLoader(GraphicsDevice));
            ResourceManager.AddLoader("SCRP", new ScriptLoader(ScriptManager, SceneManager, this.logFile));
            ResourceManager.AddLoader("STRN", new StringLoader());
            ResourceManager.AddLoader("CHRS", new CharsetLoader());
            ResourceManager.AddLoader("COST", new CostumeLoader(GraphicsDevice));
            ResourceManager.AddLoader("VERB", new VerbLoader());
            ResourceManager.AddLoader("OBJC", new ObjectLoader());

            // Create font
            font = Content.Load<SpriteFont>("Scumm.PCContent/Font");

            // Read game files
            ResourceManager.LoadGame();

            this.ScriptManager.Run(0);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
