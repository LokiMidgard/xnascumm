using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scumm.Engine.Resources.Scripts
{
    public class ScriptV5 : Script
    {
        private Action[] opCodeHandlers;
        private int currentOpCode;

        public ScriptV5(string resourceId, byte[] data)
            : base(resourceId, data)
        {
            this.opCodeHandlers = new Action[300];
        }

        public override void ExecuteInstruction()
        {
            this.currentOpCode = this.DataReader.ReadByte();

            if (this.opCodeHandlers[this.currentOpCode] != null)
            {
                this.opCodeHandlers[this.currentOpCode]();
            }
            else if (this.currentOpCode != 0xFF)
            {
                Console.Write("Script Engine", "Unknown opcode '0x{0:X2}'.", this.currentOpCode);
            }
        }
    }
}
