﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Scumm.Engine.Resources.Scripts
{
    public class ScriptManager
    {
        ResourceManager resourceManager;

        public ScriptManager(ResourceManager manager)
        {
            this.VirtualMachineStack = new Stack<int>();
            this.Variables = new Dictionary<uint, object>();
            this.BitVariables = new Dictionary<uint, uint>();
            this.ActiveScripts = new List<Script>();

            resourceManager = manager;

            // Initialize variables
            WriteVariable((uint)VariableV5.VAR_VERSION, 21);
            WriteVariable((uint)VariableV5.VAR_DEBUGMODE, 1);

            WriteVariable((uint)VariableV5.VAR_CURRENTDRIVE, 0);
            WriteVariable((uint)VariableV5.VAR_FIXEDDISK,    true);
            WriteVariable((uint)VariableV5.VAR_SOUNDCARD,    0);
            WriteVariable((uint)VariableV5.VAR_VIDEOMODE,    19);
            WriteVariable((uint)VariableV5.VAR_HEAPSPACE,    0);
            WriteVariable((uint)VariableV5.VAR_INPUTMODE,    0);
            WriteVariable((uint)VariableV5.VAR_SOUNDPARAM,   0);
            WriteVariable((uint)VariableV5.VAR_SOUNDPARAM2,  0);
            WriteVariable((uint)VariableV5.VAR_SOUNDPARAM3,  0);
        }

        public IList<Script> ActiveScripts
        {
            get;
            private set;
        }

        public IDictionary<uint, uint> BitVariables
        {
            get;
            private set;
        }

        public IDictionary<uint, object> Variables
        {
            get;
            private set;
        }

        public Stack<int> VirtualMachineStack
        {
            get;
            private set;
        }

        public void Run(int? bootParameter)
        {
            // Run the boot script
            var script = resourceManager.Load<Script>("SCRP", (byte)1);
                        
            if (bootParameter.HasValue)
            {
                script.Run(new int[] { bootParameter.Value });
            }

            else
            {
                script.Run();
            }
        }

        public void RunActiveScripts(GameTime gameTime)
        {
            for (int i = 0; i < this.ActiveScripts.Count; i++)
            {
                var script = this.ActiveScripts[i];

                if (script.Status == ScriptStatus.Paused)
                {
                    if(script.Delay > 0)
                        script.Delay -= gameTime.ElapsedGameTime.TotalSeconds * 60;
                    else
                        script.Continue();
                }
            }
        }

        public void RunScript(int scriptId, int[] arguments)
        {
            // TODO: Implement slot system for multi threaded script execution at each frame
            var script = resourceManager.Load<Script>("SCRP", (byte)scriptId);

            this.ActiveScripts.Add(script);
            script.Run(arguments);
        }

        public void FreezeScripts(byte flag, string currentScript)
        {
            for (int i = 0; i < ActiveScripts.Count; ++i)
            {
                if(String.Format("SCRP_{0}", i) == currentScript)
                    continue;
                if (ActiveScripts[i].Status != ScriptStatus.Running)// && (ActiveScripts[i].unk1 || flag >= 0x80))
                    ActiveScripts[i].Status |= ScriptStatus.Frozen;
            }
        }

        public void UnfreezeScripts()
        {
            for (int i = 0; i < ActiveScripts.Count; ++i)
            {
                ActiveScripts[i].Status &= ~ScriptStatus.Frozen;
            }
        }

        public object ReadVariable(uint variableAddress, Script script)
        {
            // Check to see if the variable is global
            if ((variableAddress & 0xF000) == 0)
            {
                if(!(this.Variables.ContainsKey(variableAddress)))
                    this.Variables[variableAddress] = 0;

                return this.Variables[variableAddress];
            }

            // Check to see if the variable is a bit variable
            if ((variableAddress & 0x8000) != 0)
            {
                if(!(this.BitVariables.ContainsKey(variableAddress & 0x7FFF)))
                    this.BitVariables[variableAddress & 0x7FFF] = 0;

                var offset = (byte)variableAddress & 0x7FFF;

                if(this.BitVariables.ContainsKey((uint)(offset >> 3)))
                {
                    return ((int)this.BitVariables[(uint)(offset >> 3)] & (1 << (offset & 7))) != 0 ? 1 : 0;
                }
            }

            // Check to see if the variable is local
            if ((variableAddress & 0x4000) != 0)
            {
                return script.ReadLocalVariable(variableAddress & 0xFFF);
            }

            // Should never happen
            return 0;
        }

        public void WriteVariable(uint variableAddress, object value)
        {
            this.WriteVariable(variableAddress, value, null);
        }

        public void WriteVariable(uint variableAddress, object value, Script script)
        {
            // Check to see if the variable is global
            if ((variableAddress & 0xF000) == 0)
            {
                this.Variables[variableAddress] = value;
            }

            // Check to see if the variable is a bit variable
            else if ((variableAddress & 0x8000) != 0)
            {
                var offset = (byte)variableAddress & 0x7FFF;

                this.BitVariables[(uint)(offset >> 3)] = 0;

                if (Convert.ToInt32(value) != 0)
                {
                    this.BitVariables[(uint)(offset >> 3)] = this.BitVariables[(uint)(offset >> 3)] | (uint)(1 << (offset & 7));
                }

                else
                {
                    this.BitVariables[(uint)(offset >> 3)] = this.BitVariables[(uint)(offset >> 3)] & (uint)~(1 << (offset & 7));
                }
            }

            // Check to see if the variable is local
            else if ((variableAddress & 0x4000) != 0)
            {
                script.WriteLocalVariable(variableAddress & 0xFFF, Convert.ToInt32(value));
            }
        }

        public void DeleteVariable(uint variableAddress)
        {
            if (this.Variables.ContainsKey(variableAddress))
            {
                this.Variables.Remove(variableAddress);
            }
        }

        public int ReadArrayValue(uint arrayAddress, int index, int baseOffset)
        {
            if (!this.Variables.ContainsKey(arrayAddress))
            {
                return 0;
            }

            if (this.Variables[arrayAddress] is int[,])
            {
                return ((int[,])this.Variables[arrayAddress])[baseOffset, index];
            }

            else if (this.Variables[arrayAddress] is byte[,])
            {
                return ((byte[,])this.Variables[arrayAddress])[baseOffset, index];
            }

            else
            {
                throw new InvalidOperationException("Invalid array type.");
            }
        }

        public void WriteArrayValue(uint arrayAddress, int index, int baseOffset, int value)
        {
            if (!this.Variables.ContainsKey(arrayAddress))
            {
                return;
            }

            if (this.Variables[arrayAddress] is string)
            {
                var charArray = ((string)this.Variables[arrayAddress]).ToCharArray();
                charArray[baseOffset + index] = (char)value;
                this.Variables[arrayAddress] = new string(charArray);
            }

            else
            {
                var array = (Array)this.Variables[arrayAddress];

                if (this.Variables[arrayAddress] is int[,])
                {
                    ((int[,])this.Variables[arrayAddress])[baseOffset, index] = value;
                }

                else if (this.Variables[arrayAddress] is byte[,])
                {
                    ((byte[,])this.Variables[arrayAddress])[baseOffset, index] = (byte)value;
                }

                else
                {
                    throw new InvalidOperationException("Invalid array type.");
                }
            }
        }

        public void DefineArray(uint arrayAddress, ArrayType type, int dimension2, int dimension1)
        {
            if (type == ArrayType.IntArray)
            {
                this.WriteVariable(arrayAddress, new int[dimension1 + 1, dimension2 + 1]);
            }

            else
            {
                this.WriteVariable(arrayAddress, new byte[dimension1 + 1, dimension2 + 1]);
            }
        }

        // Aliasing for methods
        public int Pop()
        {
            return VirtualMachineStack.Pop();
        }
        public void Push(int value)
        {
            VirtualMachineStack.Push(value);
        }

    }
}
