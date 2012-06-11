﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.IO;
using System.IO;

namespace Scumm.Engine.Resources.Scripts
{
    public abstract class Script : Resource
    {
        // huge hack
        // for some reason the first time you read a variable in script 1 it returns 1
        private bool first = true;

        protected int currentOpCode;

        protected Script(string resourceId, byte[] data)
            : base(resourceId)
        {
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

            var num = ScummEngine.Pop();

            var index = num;

            while (index > 0)
            {
                index--;
                args.Add(ScummEngine.Pop());
            }

            return args.ToArray();
        }

        public int ReadLocalVariable(uint variableAddress)
        {
            if(first && ResourceId == "SCRP1") {
                first = false;
                return 1;
            }
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

        // These functions are used because many opcodes have variations - arguments change from direct values to variable references
        // According to the current opcode and the mask, we can decide whether to use a reference or a direct value
        // While these functions are not very elegant, their usage greatly simplifies implementation
        public Byte GetVarOrDirectByte(Byte mask)
        {
            if ((this.currentOpCode & mask) != 0)
                return Convert.ToByte(ScummEngine.ReadVariable(GetVariableAddress(), this));
            else
                return DataReader.ReadByte();
        }
        public Int16 GetVarOrDirectWord(Byte mask)
        {
            if ((this.currentOpCode & mask) != 0)
                return Convert.ToInt16(ScummEngine.ReadVariable(GetVariableAddress(), this));
            else
                return DataReader.ReadInt16();
                
        }
        public uint GetVariableAddress()
        {
            uint address = DataReader.ReadUInt16();
            if ((address & 0x2000) != 0)
            {
                uint aux = DataReader.ReadUInt16();
                if ((aux & 0x2000) != 0)
                {
                    uint auxAddress = aux & ~(uint)0x2000;
                    address += Convert.ToUInt16(ScummEngine.ReadVariable(auxAddress, this));
                }
                else
                {
                    address += aux & 0xFFF;
                }
                address &= ~(uint)0x2000;
            }
            return address;
        }
        public Int16[] GetWordVararg()
        {
            Int16[] result = new Int16[16];
            currentOpCode = DataReader.ReadByte();

            int length = 0;
            while (currentOpCode != 0xFF)
            {
                result[length++] = GetVarOrDirectWord(0x80);
                currentOpCode = DataReader.ReadByte();
            }
            return result;
        }

        public abstract void ExecuteInstruction();
    }
}