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

namespace Scumm.Engine
{
    public class ScummEngine : Game
    {
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
        public static ScummEngine Instance
        {
            get { return instance; }
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
            if (instance != null)
            {
                throw new InvalidOperationException("A scumm engine instance already exists.");
            }
            instance = this;

            // Log file
            logFile = new StreamWriter("logFileMy.txt");

            // Initialize the scene manager
            this.SceneManager = new SceneManager(this);
            Components.Add(SceneManager);

            this.IsMouseVisible = true;
            this.ResourceManager = new ResourceManager(gamePath, gameId, 5);

            // Initialize script manager
            this.ScriptManager = new ScriptManager();           
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

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

        // Aliasing for methods
        static public void WriteVariable(uint variableAddress, object value, Script script)
        {
            Instance.ScriptManager.WriteVariable(variableAddress, value, script);
        }
        static public object ReadVariable(uint variableAddress, Script script)
        {
            return Instance.ScriptManager.ReadVariable(variableAddress, script);
        }
        static public int Pop()
        {
            return Instance.ScriptManager.VirtualMachineStack.Pop();
        }
        static public void Push(int value)
        {
            Instance.ScriptManager.VirtualMachineStack.Push(value);
        }
    }
}
