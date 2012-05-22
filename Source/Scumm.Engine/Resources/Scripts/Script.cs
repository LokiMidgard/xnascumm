using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.IO;
using System.IO;

namespace Scumm.Engine.Resources.Scripts
{
    public abstract class Script : Resource
    {
        protected Script(string resourceId, byte[] data)
        {
            // TODO: Remove this, debug only
            //File.WriteAllBytes("Dump\\" + resourceId, data);

            this.DataReader = new ScummBinaryReader(new MemoryStream(data));
            this.Status = ScriptStatus.Stopped;
        }

        private int[] LocalVariables
        {
            get;
            set;
        }

        public ScummBinaryReader DataReader
        {
            get;
            internal set;
        }

        public double Delay
        {
            get;
            internal set;
        }

        public ScriptStatus Status
        {
            get;
            protected set;
        }

        public void Continue()
        {
            this.Run(new int[0], false);
        }

        public void Run()
        {
            this.Run(new int[0]);
        }

        public void Run(int[] arguments)
        {
            this.Run(arguments, true);
        }

        public void Run(int[] arguments, bool resetScriptCursor)
        {
            this.Status = ScriptStatus.Running;

            // Init local variables
            this.LocalVariables = new int[25];

            if (arguments.Length > 0)
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    this.LocalVariables[i] = arguments[i];
                }
            }

            // Run the script
            if (resetScriptCursor)
            {
                this.DataReader.BaseStream.Position = 0;
            }

            while (this.DataReader.BaseStream.Position < this.DataReader.BaseStream.Length && this.Status == ScriptStatus.Running)
            {
                this.ExecuteInstruction();
            }

            if (this.Status == ScriptStatus.Running)
            {
                this.Status = ScriptStatus.Stopped;
            }
        }

        protected int[] GetStackList()
        {
            var args = new List<int>();

            var num = ScummEngine.Instance.ScriptManager.VirtualMachineStack.Pop();

            var index = num;

            while (index > 0)
            {
                index--;
                args.Add(ScummEngine.Instance.ScriptManager.VirtualMachineStack.Pop());
            }

            return args.ToArray();
        }

        public int ReadLocalVariable(uint variableAddress)
        {
            if (variableAddress < 0 || variableAddress >= 25)
            {
                throw new IndexOutOfRangeException("Local variable address was out of range.");
            }

            return this.LocalVariables[variableAddress];
        }

        public void Stop()
        {
            this.Status = ScriptStatus.Stopped;
            ScummEngine.Instance.ScriptManager.ActiveScripts.Remove(this);
        }

        public void WriteLocalVariable(uint variableAddress, int value)
        {
            if (variableAddress < 0 || variableAddress >= 25)
            {
                throw new IndexOutOfRangeException("Local variable address was out of range.");
            }

            this.LocalVariables[variableAddress] = value;
        }

        public abstract void ExecuteInstruction();
    }
}
