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
        //private static ScummEngine instance;
      
        private SceneManager sceneManager;
        private EventManager eventManager;
        private ResourceManager resourceManager;
        private ScriptManager scriptManager;
        private ScummStateV5 scummState;

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
        public ScummStateV5 ScummState
        {
            get { return scummState; }
            set { scummState = value; }
        }
                
        public ScummEngine(string gamePath, string gameId, int scummVersion)
        {
            // Log file
            logFile = new StreamWriter("Scumm.log");
            logFile.AutoFlush = true;

            // Initialize state
            ScummState = new ScummStateV5();

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
            ResourceManager.AddLoader("RMIM", new RoomImageLoader(GraphicsDevice));
            ResourceManager.AddLoader("OBIM", new ObjectImageLoader(GraphicsDevice));
            ResourceManager.AddLoader("SCRP", new ScriptLoader(ScriptManager, SceneManager, ScummState, this.logFile));
            ResourceManager.AddLoader("STRN", new StringLoader());
            ResourceManager.AddLoader("CHRS", new CharsetLoader(GraphicsDevice));
            ResourceManager.AddLoader("COST", new CostumeLoader(GraphicsDevice));
            ResourceManager.AddLoader("VERB", new VerbLoader());
            ResourceManager.AddLoader("OBJC", new ObjectLoader());

            // Read game files
            ResourceManager.LoadGame();

            this.ScriptManager.Run(456);
        }
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            int delta = ((short)this.scriptManager.Variables[(uint)VariableV5.VAR_TIMER_NEXT] != 0xFF) ? (short)this.scriptManager.Variables[(uint)VariableV5.VAR_TIMER_NEXT] : 4;
            var deltaTimeSpan = TimeSpan.FromMilliseconds((float)delta * 1000 / 60);
            //deltaTimeSpan = TimeSpan.FromMilliseconds(500);
            if (this.TargetElapsedTime != deltaTimeSpan)
            {
                this.TargetElapsedTime = deltaTimeSpan;
            }

            // Notify the script about how much time has passed, in ticks (60 ticks per second)
            if (this.scriptManager.Variables.ContainsKey((uint)VariableV5.VAR_TIMER) && (int)this.scriptManager.Variables[(uint)VariableV5.VAR_TIMER] != 0xFF)
            {
                this.scriptManager.Variables[(uint)VariableV5.VAR_TIMER] = delta;
            }

            if (this.scriptManager.Variables.ContainsKey((uint)VariableV5.VAR_TIMER_TOTAL) && (int)this.scriptManager.Variables[(uint)VariableV5.VAR_TIMER_TOTAL] != 0xFF)
            {
                this.scriptManager.Variables[(uint)VariableV5.VAR_TIMER_TOTAL] = (int)this.scriptManager.Variables[(uint)VariableV5.VAR_TIMER_TOTAL] + delta;
            }

            base.Update(gameTime);

            scriptManager.RunActiveScripts(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
