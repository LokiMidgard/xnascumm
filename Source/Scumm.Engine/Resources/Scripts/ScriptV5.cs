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

            this.opCodeHandlers[26] = new Action(OpCodeWriteWordToVariable);
            this.opCodeHandlers[37] = new Action(OpCodePickupObject);
            this.opCodeHandlers[39] = new Action(OpCodeStringCommand);
            this.opCodeHandlers[44] = new Action(OpCodeCursorCommand);
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
                Console.WriteLine("Unknown opcode {0}", this.currentOpCode);
            }
        }

        private void OpCodeWriteWordToVariable()
        {
            var variableAddress = this.DataReader.ReadUInt16();
            var variableValue = this.DataReader.ReadUInt16();

            ScummEngine.Instance.ScriptManager.WriteVariable(variableAddress, variableValue, this);
        }

        private void OpCodePickupObject()
        {

        }

        private void OpCodeCursorCommand()
        {
            var subCode = this.DataReader.ReadByte();
            switch (subCode)
            {
                // show cursor
                case 1:
                    ScummEngine.Instance.ScriptManager.WriteVariable((uint)VariableV5.VAR_CURSORSTATE, 1, this);
                    break;
                // hide cursor
                case 2:
                    ScummEngine.Instance.ScriptManager.WriteVariable((uint)VariableV5.VAR_CURSORSTATE, 0, this);
                    break;
                case 4:
                    ScummEngine.Instance.ScriptManager.WriteVariable((uint)VariableV5.VAR_USERPUT, 0, this);
                    break;
            };
        }

        private void OpCodeStringCommand()
        {
            var subCode = this.DataReader.ReadByte();
            switch (subCode)
            {
                // load string
                case 1:
                    var id = this.DataReader.ReadByte();
                    ScummEngine.Instance.ResourceManager.Load<ScummString>("STRN", id, this.DataReader);
                    break;
                case 2:
                    break;
                case 4:
                    break;
            };
        }
    }
}
