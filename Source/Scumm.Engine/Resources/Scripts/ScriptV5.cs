﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.Resources.Graphics;
using System.IO;
namespace Scumm.Engine.Resources.Scripts
{
    public class ScriptV5 : Script
    {
        static int instructionCount = 0;

        private Action[] opCodeHandlers;
        private uint currentInstructionOffset;

        public ScriptV5(string resourceId, byte[] data, ScriptManager scriptMngr, ResourceManager resourceMngr, SceneManager sceneMngr, StreamWriter logFile)
            : base(resourceId, data, scriptMngr, resourceMngr, sceneMngr, logFile)
        {
            opCodeHandlers = new Action[256];

            opCodeHandlers[0x00] = new Action(OpStopObjectCode);
            opCodeHandlers[0x01] = new Action(OpPutActor);
            opCodeHandlers[0x08] = new Action(OpIsNotEqual);
            opCodeHandlers[0x0A] = new Action(OpRunScript);
            opCodeHandlers[0x0C] = new Action(OpResourceCommand);
            opCodeHandlers[0x10] = new Action(OpGetObjectOwner);
            opCodeHandlers[0x11] = new Action(OpAnimateActor);
            opCodeHandlers[0x13] = new Action(OpActorCommand);
            opCodeHandlers[0x14] = new Action(OpPrint);
            opCodeHandlers[0x18] = new Action(OpJumpRelative);
            opCodeHandlers[0x19] = new Action(OpDoSentence);
            opCodeHandlers[0x1A] = new Action(OpMove);
            opCodeHandlers[0x25] = new Action(OpPickupObject);
            opCodeHandlers[0x26] = new Action(OpSetVariableRange);
            opCodeHandlers[0x27] = new Action(OpStringCommand);
            opCodeHandlers[0x28] = new Action(OpEqualZero);
            opCodeHandlers[0x29] = new Action(OpSetOwnerOf);
            opCodeHandlers[0x2A] = new Action(OpRunScript);
            opCodeHandlers[0x2C] = new Action(OpCursorCommand);
            opCodeHandlers[0x2D] = new Action(OpPutActorInRoom);
            opCodeHandlers[0x30] = new Action(OpMatrixCommand);
            opCodeHandlers[0x33] = new Action(OpRoomCommand);
            opCodeHandlers[0x37] = new Action(OpStartObject);
            opCodeHandlers[0x38] = new Action(OpIsLessOrEqual);
            opCodeHandlers[0x44] = new Action(OpIsLess);
            opCodeHandlers[0x46] = new Action(OpIncrement);
            opCodeHandlers[0x48] = new Action(OpIsEqual);
            opCodeHandlers[0x5A] = new Action(OpAdd);
            opCodeHandlers[0x5D] = new Action(OpActorSetClass);
            opCodeHandlers[0x68] = new Action(OpGetScriptRunning);
            opCodeHandlers[0x72] = new Action(OpLoadRoom);
            opCodeHandlers[0x78] = new Action(OpIsGreater);
            opCodeHandlers[0x7A] = new Action(OpVerbCommand);
            opCodeHandlers[0x80] = new Action(OpBreakHere);
            opCodeHandlers[0x81] = new Action(OpPutActor);
            opCodeHandlers[0x88] = new Action(OpIsNotEqual);
            opCodeHandlers[0x8B] = new Action(OpGetVerbEntryPoint);
            opCodeHandlers[0x91] = new Action(OpAnimateActor);
            opCodeHandlers[0x9A] = new Action(OpMove);
            opCodeHandlers[0xA0] = new Action(OpStopObjectCode);
            opCodeHandlers[0xA8] = new Action(OpNotEqualZero);
            opCodeHandlers[0xAC] = new Action(OpExpression);
            opCodeHandlers[0xAD] = new Action(OpPutActorInRoom);
            opCodeHandlers[0xB1] = new Action(OpGetInventoryCount);
            opCodeHandlers[0xB7] = new Action(OpStartObject);
            opCodeHandlers[0xB8] = new Action(OpIsLessOrEqual);
            opCodeHandlers[0xC4] = new Action(OpIsLess);
            opCodeHandlers[0xCC] = new Action(OpPseudoRoom);
            opCodeHandlers[0xD2] = new Action(OpActorFollowCamera);
            opCodeHandlers[0xD5] = new Action(OpActorFromPos);
            opCodeHandlers[0xE8] = new Action(OpGetScriptRunning);
            opCodeHandlers[0xF5] = new Action(OpFindObject);
            opCodeHandlers[0xF8] = new Action(OpIsGreater);
            opCodeHandlers[0xFA] = new Action(OpVerbCommand);
            opCodeHandlers[0xFD] = new Action(OpFindInventory); 
        }

        public override void ExecuteInstruction()
        {
            this.currentInstructionOffset = (uint)DataReader.BaseStream.Position;
            currentOpCode = DataReader.ReadByte();

            if (opCodeHandlers[currentOpCode] != null)
            {
                try
                {
                    this.LogOpCodeInformations("{0}. {1} : {2}", ++instructionCount, ResourceId, currentOpCode);
                    opCodeHandlers[currentOpCode]();
                }

				catch (Exception e)
				{
                    Console.WriteLine("ERROR: {0}", e.Message);
                    #if !COMPARE
                    this.LogOpCodeInformations("ERROR: {0}", e.Message);
                    #endif
				}
            }

            else if (currentOpCode != 0xFF)
            {
                Console.WriteLine("Unknown opcode {0}", currentOpCode);
                this.LogOpCodeInformations("Unknown opcode {0}", currentOpCode);
            }
        }

        private void StartScene(byte roomId)
        {
            scriptManager.WriteVariable((uint)VariableV5.VAR_NEW_ROOM, roomId, this);
            RunExitScript(scriptManager.CurrentRoomId);

            scriptManager.WriteVariable((uint)VariableV5.VAR_ROOM, roomId, this);
            scriptManager.WriteVariable((uint)VariableV5.VAR_ROOM_RESOURCE, roomId, this);

            scriptManager.CurrentRoomId = roomId;

            // provisory?!
            if (roomId == 0)
                return;

            // update scene manager
            sceneManager.CurrentRoom = resourceManager.Load<Room>("ROOM", roomId);
            for (int i = 0; i < 13; ++i)
            {
                Actor actor = resourceManager.FindActor(i);
                if (actor.RoomID == roomId)
                    sceneManager.CurrentActors.Add(actor);
            }

            RunEntryScript(roomId);
        }

        private void RunExitScript(byte roomId)
        {
            Byte scriptId = Convert.ToByte(scriptManager.ReadVariable((uint)VariableV5.VAR_EXIT_SCRIPT, this));
            if (scriptId != 0)
            {
                Script exit = resourceManager.Load<Script>("SCRP", scriptId);
                exit.Run();
            }

            if (roomId != 0)
            {
                Room room = resourceManager.Load<Room>("ROOM", roomId);
                if (room.ExitScript != null)
                    room.ExitScript.Run();
            }

            scriptId = Convert.ToByte(scriptManager.ReadVariable((uint)VariableV5.VAR_EXIT_SCRIPT2, this));
            if (scriptId != 0)
            {
                Script exit = resourceManager.Load<Script>("SCRP", scriptId);
                exit.Run();
            }
        }

        private void RunEntryScript(byte roomId)
        {
            Byte scriptId = Convert.ToByte(scriptManager.ReadVariable((uint)VariableV5.VAR_ENTRY_SCRIPT, this));
            if (scriptId != 0)
            {
                Script entry = resourceManager.Load<Script>("SCRP", scriptId);
                entry.Run();
            }

            if (roomId != 0)
            {
                Room room = resourceManager.Load<Room>("ROOM", roomId);
                if (room.EntryScript != null)
                    room.EntryScript.Run();
            }

            scriptId = Convert.ToByte(scriptManager.ReadVariable((uint)VariableV5.VAR_ENTRY_SCRIPT2, this));
            if (scriptId != 0)
            {
                Script entry = resourceManager.Load<Script>("SCRP", scriptId);
                entry.Run();
            }
        }

        private void RunVerbCode(int objId, byte entry, int a, int b, int[] data)
        {
            if(objId == 0)
                return;

            Object obj = resourceManager.FindObject(objId);
            Script script = obj.VerbScript[entry];
            script.Run(data);
        }

        private void RunHook(byte hookId)
        {
            Byte scriptId = Convert.ToByte(scriptManager.ReadVariable((uint)VariableV5.VAR_INVENTORY_SCRIPT, this));
            if (scriptId != 0)
            {
                Script hook = resourceManager.Load<Script>("SCRP", scriptId);

                int[] localVars = new int[1];
                localVars[0] = hookId;
                hook.Run(localVars);
            }
        }

        private void DecodeParseString(int actorId)
        {
            int textSlot;

            switch (actorId)
            {
                case 252:
                    textSlot = 3;
                    break;
                case 253:
                    textSlot = 2;
                    break;
                case 254:
                    textSlot = 1;
                    break;
                default:
                    textSlot = 0;
                    break;
            }

            //_stringXpos[textSlot] = textslot.x[textSlot];
            //_stringYpos[textSlot] = textslot.y[textSlot];
            //_stringCenter[textSlot] = textslot.center[textSlot];
            //_stringOverhead[textSlot] = textslot.overhead[textSlot];
            //_stringRight[textSlot] = textslot.right[textSlot];
            //_stringColor[textSlot] = textslot.color[textSlot];
            //_stringCharset[textSlot] = textslot.charset[textSlot];

            currentOpCode = DataReader.ReadByte();
            while (currentOpCode != 0xFF)
            {
                switch (currentOpCode)
                {
                    case 0: /* set string xy */
                        GetVarOrDirectWord(0x80, currentOpCode);
                        GetVarOrDirectWord(0x40, currentOpCode);
                        //_stringXpos[textSlot] = getVarOrDirectWord(0x80);
                        //_stringYpos[textSlot] = getVarOrDirectWord(0x40);
                        //_stringOverhead[textSlot] = 0;
                        break;
                    case 1: /* color */
                        GetVarOrDirectByte(0x80, currentOpCode);
                        //_stringColor[textSlot] = 
                        break;
                    case 2: /* right */
                        GetVarOrDirectByte(0x80, currentOpCode);
                        //_stringRight[textSlot] = getVarOrDirectWord(0x80);
                        break;
                    case 4:	/* center*/
                        //_stringCenter[textSlot] = 1;
                        //_stringOverhead[textSlot] = 0;
                        break;
                    case 6: /* left */
                        //_stringCenter[textSlot] = 0;
                        //_stringOverhead[textSlot] = 0;
                        break;
                    case 7: /* overhead */
                        //_stringOverhead[textSlot] = 1;
                        break;
                    case 8: /* ignore */
                        GetVarOrDirectWord(0x80, currentOpCode);
                        GetVarOrDirectWord(0x40, currentOpCode);
                        break;
                    case 15:
                        Byte b = DataReader.ReadByte();
                        switch (textSlot)
                        {
                            case 0:
                                Actor actor = resourceManager.FindActor(actorId);
                                actor.Talk(); 
                                break;
                            //case 1: drawString(1); break;
                            //case 2: unkMessage1(); break;
                            //case 3: unkMessage2(); break;
                            default:
                                break;
                        }
                        //_scriptPointer = _messagePtr;
                        return;
                    default:
                        return;
                }
                currentOpCode = DataReader.ReadByte();
            }

            //textslot.x[textSlot] = _stringXpos[textSlot];
            //textslot.y[textSlot] = _stringYpos[textSlot];
            //textslot.center[textSlot] = _stringCenter[textSlot];
            //textslot.overhead[textSlot] = _stringOverhead[textSlot];
            //textslot.right[textSlot] = _stringRight[textSlot];
            //textslot.color[textSlot] = _stringColor[textSlot];
            //textslot.charset[textSlot] = _stringCharset[textSlot];
        }

        #region Ops

        private void OpDoSentence()
        {
            Byte a = GetVarOrDirectByte(0x80, currentOpCode);
            if(a == 254)
                return;
            GetVarOrDirectWord(0x40, currentOpCode);
            GetVarOrDirectWord(0x20, currentOpCode);
        }

        private void OpMove()
        {
            uint variableAddress = GetVariableAddress();
            Int16 variableValue = GetVarOrDirectWord(0x80, currentOpCode);

            scriptManager.WriteVariable(variableAddress, variableValue, this);
            #if !COMPARE
            this.LogVariable(variableAddress, " = {0}", variableValue);
            #endif
        }

        private void OpPrint()
        {
            byte actor = GetVarOrDirectByte(0x80, currentOpCode);
            DecodeParseString(actor);
        }

        private void OpStartObject()
        {
            var obj = GetVarOrDirectWord(0x80, currentOpCode);
            var script = GetVarOrDirectByte(0x40, currentOpCode);

            short[] data = GetWordVararg();
            int[] dataInt = new int[data.Count<short>()];

            for (int i = 0; i < data.Count<short>(); ++i)
                dataInt[i] = (int)data[i];

            RunVerbCode(obj, script, 0, 0, dataInt);
        }

        private void OpSetOwnerOf()
        {
            var objId = GetVarOrDirectWord(0x80, currentOpCode);
            var owner = GetVarOrDirectByte(0x40, currentOpCode);

            if (owner == 0)
            {
            }

            Object obj = resourceManager.FindObject(objId);
            obj.setOwnerState(owner);
            RunHook(0);
        }

        private void OpPickupObject()
        {
            var objId = DataReader.ReadUInt16();
            var roomId = DataReader.ReadByte();

            if (roomId == 0)
                roomId = scriptManager.CurrentRoomId;

            Object obj = resourceManager.FindObject(objId);
            sceneManager.AddObjectToInventory(obj);
            obj.setOwnerState(1);
            RunHook(1);
        }

        private void OpGetInventoryCount()
        {
            var address = GetVariableAddress();
            var owner = GetVarOrDirectByte(0x80, currentOpCode);

            int count = 0;
            for (int i = 0; i < sceneManager.Inventory.Count; ++i)
            {
                Object obj = sceneManager.GetInventoryObject(i);
                if (obj != null && obj.getOwnerState() == owner)
                    ++count;
            }

            scriptManager.WriteVariable(address, count, this);
        }

        private void OpFindInventory()
        {
            var address = GetVariableAddress();
            var owner = GetVarOrDirectByte(0x80, currentOpCode);
            var b = GetVarOrDirectByte(0x40, currentOpCode);

            int count = 1;
            for (int i = 0; i < sceneManager.Inventory.Count; ++i)
            {
                Object obj = sceneManager.GetInventoryObject(i);
                if (obj != null && obj.getOwnerState() == owner && count++ == b)
                {
                    scriptManager.WriteVariable(address, obj.Id, this);
                    return;
                }
            }

            scriptManager.WriteVariable(address, 0, this);
        }

        private void OpGetVerbEntryPoint()
        {
            var address = GetVariableAddress();
            var a = GetVarOrDirectWord(0x80, currentOpCode);
            var b = GetVarOrDirectWord(0x40, currentOpCode);

            scriptManager.WriteVariable(address, 50, this);
        }

        private void OpGetObjectOwner()
        {
            uint variableAddress = GetVariableAddress();
            Int16 objectId = GetVarOrDirectWord(0x80, currentOpCode);
            Object obj = resourceManager.FindObject(objectId);

            scriptManager.WriteVariable(variableAddress, obj.OwnerState, this);
        }

        private void OpRunScript()
        {
            byte scriptId = GetVarOrDirectByte(0x80, currentOpCode);
            Int16[] data = GetWordVararg();

            int a, b;
            // TODO: I don't know what these are used for
            a = b = 0;
            if ((currentOpCode & 0x20) != 0) a = 1;
            if ((currentOpCode & 0x40) != 0) b = 1;

            if (scriptId < 199)
            {
#if !COMPARE
                this.LogOpCodeInformations("RunScript({0})", scriptId);
#endif
                Script script = resourceManager.Load<Script>("SCRP", scriptId);
                script.Run();
            }
            else
            {
                Room room = resourceManager.Load<Room>("ROOM", scriptManager.CurrentRoomId);
                Script script = room.Scripts[scriptId - 200];
                script.Run();
            }
        }

        private void OpGetScriptRunning()
        {
            uint address = GetVariableAddress();
            Byte script = GetVarOrDirectByte(0x80, currentOpCode);
            String scriptCode = String.Format("SCRP_{0}", script);

            for (int i = 0; i < scriptManager.ActiveScripts.Count; ++i)
            {
                if (scriptCode == scriptManager.ActiveScripts[i].ResourceId)
                {
                    scriptManager.WriteVariable(address, 1, this);
                    return;
                }
            }
            scriptManager.WriteVariable(address, 0, this);
        }

        private void OpBreakHere()
        {
            #if !COMPARE
            this.LogOpCodeInformations("StopScript()");
            #endif
            Stop();
        }

        private void OpStopObjectCode()
        {
            Stop();
        }

        private void OpPseudoRoom()
        {
            int i = DataReader.ReadByte();
            int j = DataReader.ReadByte();
            while (j != 0)
            {
                if (j >= 0x80)
                {
                    //_resourceMapper[j & 0x7F] = i;
                }
                j = DataReader.ReadByte();
            }
        }

        private void OpFindObject() 
        {
            uint address = GetVariableAddress();
            Int16 objX = GetVarOrDirectWord(0x80, currentOpCode);
            Int16 objY = GetVarOrDirectWord(0x40, currentOpCode);
	        //setResult(findObject(t, getVarOrDirectWord(0x40)));
        }

        private void OpMatrixCommand()
        {
            int a, b;

            var subOpCode = DataReader.ReadByte();

            switch (subOpCode & 0x1F)
            {
                case 1:
                    a = GetVarOrDirectByte(0x80, subOpCode);
                    b = GetVarOrDirectByte(0x40, subOpCode);
                    //setBoxFlags(a, b);
                    this.LogOpCodeInformations("SetBoxFlags()");
                    break;
                case 2:
                    a = GetVarOrDirectByte(0x80, subOpCode);
                    b = GetVarOrDirectByte(0x40, subOpCode);
                    //setBoxScale(a, b);
                    this.LogOpCodeInformations("SetBoxScale()");

                    break;
                case 3:
                    a = GetVarOrDirectByte(0x80, subOpCode);
                    b = GetVarOrDirectByte(0x40, subOpCode);
                    //setBoxScale(a, (b - 1) | 0x8000);
                    this.LogOpCodeInformations("SetBoxScale(...)");
                    break;
                case 4:
                    //createBoxMatrix();
                    this.LogOpCodeInformations("CreateBoxMatrix()");
                    break;
            }
        }

        private void OpActorFollowCamera()
        {
	        //int a = camera._follows;

            var actorId = GetVarOrDirectByte(0x80, currentOpCode);
            Actor actor = resourceManager.FindActor(actorId);

            if (actor.RoomID != scriptManager.CurrentRoomId)
                StartScene(actor.RoomID);

            //, "actorFollowCamera"));
            //
	        //if (camera._follows != a) 
		    //    runHook(0);
            //
	        //camera._movingToActor = 0;
            #if !COMPARE
            this.LogOpCodeInformations("ActorFollowCamera(Actor{0})", actorId);
            #endif
        }

        private void OpActorFromPos() 
        {
	        uint address = GetVariableAddress();
            Int16 objX = GetVarOrDirectWord(0x80, currentOpCode);
            Int16 objY = GetVarOrDirectWord(0x40, currentOpCode);
            //setResult(getActorFromPos(x,y));
        }

        private void OpActorSetClass()
        {
            var actorID = GetVarOrDirectWord(0x80, currentOpCode);
            Actor actor = resourceManager.FindActor(actorID);

            var subOpCode = DataReader.ReadByte();
            while (subOpCode != 0xFF)
            {
                int i = GetVarOrDirectWord(0x80, subOpCode);
                if (i == 0)
                {
                    actor.ClassData = 0;
                    continue;
                }
                //if ((i & 0x80) > 0)
                //    actor.ClassData = 1;
                //else
                //    actor.ClassData = 0;
            }
        }

        private void OpPutActorInRoom() 
        {
            var actorID = GetVarOrDirectByte(0x80, currentOpCode);

            Actor actor = resourceManager.FindActor(actorID);
            actor.RoomID = GetVarOrDirectByte(0x40, currentOpCode);

            #if !COMPARE
            this.LogOpCodeInformations("Actor{0}.RoomID = {1}", actorID, actor.RoomID);
            #endif
	    }

        private void OpPutActor() 
        {
            var actorID = GetVarOrDirectByte(0x80, currentOpCode);
            var x = GetVarOrDirectWord(0x40, currentOpCode);
            var y = GetVarOrDirectWord(0x20, currentOpCode);

            Actor actor = resourceManager.FindActor(actorID);
            actor.PutActor(x, y);
            #if !COMPARE
            this.LogOpCodeInformations("Actor{0}.PutActor({1}, {2})", actorID, x, y);
            #endif
        }

        private void OpAnimateActor()
        {
            GetVarOrDirectByte(0x80, currentOpCode);
            GetVarOrDirectByte(0x40, currentOpCode);
        }

        private void OpLoadRoom()
        {
            byte room = GetVarOrDirectByte(0x80, currentOpCode);

            this.LogOpCodeInformations("Loading room {0}", room);

            if (room != 0)
                resourceManager.Load<Room>("ROOM", room);

            this.scriptManager.CurrentRoomId = room;
            StartScene(room);
        }

        private void OpSetVariableRange()
        {
            uint variableId = DataReader.ReadUInt16();
            byte intervalSize = DataReader.ReadByte();
            for (int i = 0; i < intervalSize; ++i)
            {
                byte value = DataReader.ReadByte();
                scriptManager.WriteVariable(variableId, value);
                ++variableId;
            }
        }

        private void OpJumpRelative()
        {
            Int16 jump = DataReader.ReadInt16();
            #if !COMPARE
            this.LogOpCodeInformations("goto [{0:X4}]", DataReader.BaseStream.Position + jump);
            #endif
            DataReader.BaseStream.Position += jump;   
        }

        private void OpIsNotEqual()
        {
            Int16 a = Convert.ToInt16(scriptManager.ReadVariable(GetVariableAddress(), this));
            Int16 b = GetVarOrDirectWord(0x80, currentOpCode);

            Int16 jump = DataReader.ReadInt16();
            
            #if !COMPARE
            if (a != b)
            {
                this.LogOpCodeInformations("if({0} != {1})", a, b);
            }
            else
            {
                this.LogOpCodeInformations("if({0} != {1}) : FALSE => goto({2:X4})", a, b, DataReader.BaseStream.Position + jump);
            }
            #endif

            if (a == b) DataReader.BaseStream.Position += jump;   
        }         

        private void OpIsEqual()
        {
            Int16 a = Convert.ToInt16(scriptManager.ReadVariable(GetVariableAddress(), this));
            Int16 b = GetVarOrDirectWord(0x80, currentOpCode);

            Int16 jump = DataReader.ReadInt16();

            #if !COMPARE
            if (a == b)
            {
                this.LogOpCodeInformations("if({0} == {1})", a, b);
            }
            else
            {
                this.LogOpCodeInformations("if({0} == {1}) : FALSE => goto({2:X4})", a, b, DataReader.BaseStream.Position + jump);
            }
            #endif

            if (a != b) DataReader.BaseStream.Position += jump;
        }

        private void OpIsLess()
        {
            Int16 a = Convert.ToInt16(scriptManager.ReadVariable(GetVariableAddress(), this));
            Int16 b = GetVarOrDirectWord(0x80, currentOpCode);

            Int16 jump = DataReader.ReadInt16();

            #if !COMPARE
            if (b < a)
            {
                this.LogOpCodeInformations("if({0} < {1})", b, a);
            }

            else
            {
                this.LogOpCodeInformations("if({0} < {1}) : FALSE => goto({2:X4})", b, a, DataReader.BaseStream.Position + jump);
            }
            #endif

            if (b >= a) DataReader.BaseStream.Position += jump;
        }

        private void OpIsGreater()
        {
            Int16 a = Convert.ToInt16(scriptManager.ReadVariable(GetVariableAddress(), this));
            Int16 b = GetVarOrDirectWord(0x80, currentOpCode);

            Int16 jump = DataReader.ReadInt16();

            #if !COMPARE
            if (b > a)
            {
                this.LogOpCodeInformations("if({0} > {1})", b, a);
            }

            else
            {
                this.LogOpCodeInformations("if({0} > {1}) : FALSE => goto({2:X4})", b, a, DataReader.BaseStream.Position + jump);
            }
            #endif

            if (b <= a) DataReader.BaseStream.Position += jump;
        }

        private void OpIsLessOrEqual()
        {
            Int16 a = Convert.ToInt16(scriptManager.ReadVariable(GetVariableAddress(), this));
            Int16 b = GetVarOrDirectWord(0x80, currentOpCode);

            Int16 jump = DataReader.ReadInt16();

            #if !COMPARE
            if (b <= a)
            {
                this.LogOpCodeInformations("if({0} <= {1})", b, a);
            }

            else
            {
                this.LogOpCodeInformations("if({0} <= {1}) : FALSE => goto({2:X4})", b, a, DataReader.BaseStream.Position + jump);
            }
            #endif

            if (b > a) DataReader.BaseStream.Position += jump;
        }

        private void OpNotEqualZero()
        {
            Int16 value = Convert.ToInt16(scriptManager.ReadVariable(GetVariableAddress(), this));
            Int16 jump = DataReader.ReadInt16();

            #if !COMPARE
            if (value != 0)
            {
                this.LogOpCodeInformations("if({0} != 0)", value);
            }

            else
            {
                this.LogOpCodeInformations("if({0} != 0) : FALSE => goto({1:X4})", value, DataReader.BaseStream.Position + jump);
            }
            #endif

            if (value == 0) DataReader.BaseStream.Position += jump;
        }

        private void OpEqualZero()
        {
            Int16 value = Convert.ToInt16(scriptManager.ReadVariable(GetVariableAddress(), this));
            Int16 jump = DataReader.ReadInt16();

            #if !COMPARE
            if (value == 0)
            {
                this.LogOpCodeInformations("if({0} == 0)", value);
            }

            else
            {
                this.LogOpCodeInformations("if({0} == 0) : FALSE => goto({1:X4})", value, DataReader.BaseStream.Position + jump);
            }
            #endif

            if (value != 0) DataReader.BaseStream.Position += jump;
        }

        private void OpAdd()
        {
            uint address = GetVariableAddress();
            Int16 a = GetVarOrDirectWord(0x80, currentOpCode);
            Int16 b = Convert.ToInt16(scriptManager.ReadVariable(address, this));

            scriptManager.WriteVariable(address, b + a, this);

            #if !COMPARE
            this.LogVariable(address, " = {0} + {1}", a, b);
            #endif
        }

        private void OpIncrement()
        {
            uint address = GetVariableAddress();
            Int16 variableValue = Convert.ToInt16(scriptManager.ReadVariable(address, this));

            scriptManager.WriteVariable(address, variableValue + 1, this);
            #if !COMPARE
            this.LogVariable(address, "++ (OldValue = {0})", variableValue);
            #endif
        }

        private void OpActorCommand()
        {
            Byte actorId = GetVarOrDirectByte(0x80, currentOpCode);
            Actor actor = resourceManager.FindActor(actorId);

            var subOpCode = DataReader.ReadByte();

            while (subOpCode != 0xFF)
            {
                switch (subOpCode)
                {
                    // costume
                    case 1:
                        int costumeId = GetVarOrDirectByte(0x80, subOpCode);
                        #if !COMPARE
                        this.LogOpCodeInformations("Actor_{0}.CostumeID = {1}", actorId, costumeId);
                        #endif
                        actor.Costume = resourceManager.Load<Costume>("COST", (int)costumeId, new Dictionary<string, object>() { { "RoomId", scriptManager.CurrentRoomId } });
                        break;
                    case 2: /* walkspeed */
                        GetVarOrDirectByte(0x80, subOpCode);
                        GetVarOrDirectByte(0x40, subOpCode);
                        //i = 
                        //j = 
                        //setActorWalkSpeed(a, i, j);
                        break;
                    case 3: /* sound */
                        GetVarOrDirectByte(0x80, subOpCode);
                        //a->sound = getVarOrDirectByte(0x80);
                        break;
                    case 4: /* walkanim */
                        GetVarOrDirectByte(0x80, subOpCode);
                        //a->walkFrame = getVarOrDirectByte(0x80);
                        break;
                    case 5: /* talkanim */
                        GetVarOrDirectByte(0x80, subOpCode);
                        GetVarOrDirectByte(0x40, subOpCode);
                        //a->talkFrame1 = getVarOrDirectByte(0x80);
                        //a->talkFrame2 = getVarOrDirectByte(0x40);
                        break;
                    case 6: /* standanim */
                        GetVarOrDirectByte(0x80, subOpCode);
                        //a->standFrame = getVarOrDirectByte(0x80);
                        break;
                    case 7: /* ignore */
                        GetVarOrDirectByte(0x80, subOpCode);
                        GetVarOrDirectByte(0x40, subOpCode);
                        GetVarOrDirectByte(0x20, subOpCode);
                        break;
                    // Init
                    case 8: 
                        actor.Init(0);
                        break;

                    case 9: /* elevation */
                        GetVarOrDirectWord(0x80, subOpCode);
                        //a->elevation = 
                        //a->needRedraw = true;
                        //a->needBgReset = true;
                        break;
                    case 10: /* defaultanims */
                        //a->initFrame = 1;
                        //a->walkFrame = 2;
                        //a->standFrame = 3;
                        //a->talkFrame1 = 4;
                        //a->talkFrame2 = 4;
                        break;
                    case 11: /* palette */
                        GetVarOrDirectByte(0x80, subOpCode);
                        GetVarOrDirectByte(0x40, subOpCode);
                        //checkRange(32, 0, i, "Illegal palet slot %d");
                        //a->palette[i] = j;
                        //a->needRedraw = 1;
                        break;
                    case 12: /* talk color */
                        //a->talkColor = getVarOrDirectByte(0x80);
                        GetVarOrDirectByte(0x80, subOpCode);
                        break;
                    case 13: /* name */
                        resourceManager.Load<ScummString>("STRN", 0, DataReader);
                        break;
                    case 14: /* initanim */
                        GetVarOrDirectByte(0x80, subOpCode);
                        //a->initFrame = 
                        break;
                    case 16: /* width */
                        GetVarOrDirectByte(0x80, subOpCode);
                        //a->width = 
                        break;
                    case 17: /* scale */
                        GetVarOrDirectByte(0x80, subOpCode);
                        GetVarOrDirectByte(0x40, subOpCode);
                        //a->scalex = 
                        //a->scaley = 
                        break;
                    case 18: /* neverzclip */
                        //a->neverZClip = 0;
                        break;
                    case 19: /* setzclip */
                        //a->neverZClip = 
                        GetVarOrDirectByte(0x80, subOpCode);
                        break;
                    case 20: /* ignoreboxes */
                        //a->ignoreBoxes = 1;
                        //a->neverZClip = 0;
                        //FixRoom:
                        //if (a->room == _currentRoom)
                        //  opPutActor(a, a->x, a->y, a->room);
                        break;
                    case 21: /* followboxes */
                    //a->ignoreBoxes = 0;
                    //a->neverZClip = 0;
                    //goto FixRoom;

                    case 22: /* animspeed */
                        //a->animSpeed = 
                        GetVarOrDirectByte(0x80, subOpCode);
                        break;
                    case 23: /* unk2 */
                        //a->data8 = 
                        GetVarOrDirectByte(0x80, subOpCode); /* unused */
                        break;
                    default:
                        throw new NotImplementedException("ActorCommand subCode not found");
                }
                subOpCode = DataReader.ReadByte();
            }
        }

        private void OpRoomCommand()
        {
            var subOpCode = DataReader.ReadByte();

            Int16 a, b, c, d, e;
            switch (subOpCode)
            {
                // Room scroll
                case 1:
                    a = GetVarOrDirectWord(0x80, subOpCode);
                    b = GetVarOrDirectWord(0x40, subOpCode);
                    //if (a < 160) a=160;
                    //if (a > ((_scrWidthIn8Unit-20)<<3)) a=((_scrWidthIn8Unit-20)<<3);
                    //if (b < 160) b=160;
                    //if (b > ((_scrWidthIn8Unit-20)<<3)) b=((_scrWidthIn8Unit-20)<<3);
                    //vm.vars[VAR_CAMERA_MIN] = a;
                    //vm.vars[VAR_CAMERA_MAX] = b;
                    break;
                // Room color
                case 2:
                    throw new InvalidCastException("Room color is no longer a valid command");
                // Set screen
                case 3:
                    a = GetVarOrDirectWord(0x80, subOpCode);
                    b = GetVarOrDirectWord(0x40, subOpCode);
                    //initScreens(0,a,320,b);
                    break;
                // Set palette color
                case 4:
                case 228:
                    a = GetVarOrDirectWord(0x80, subOpCode);
                    b = GetVarOrDirectWord(0x40, subOpCode);
                    c = GetVarOrDirectWord(0x20, subOpCode);
                    subOpCode = DataReader.ReadByte();
                    d = GetVarOrDirectByte(0x80, subOpCode);
                    //setPalColor(d, a, b, c); /* index, r, g, b */
                    break;
                case 10: // ?
                    a = GetVarOrDirectWord(0x80, subOpCode);
                    break;
                default:
                    throw new NotImplementedException("RoomCommand subCode not found");
                //case 5: /* shake on */
                //    setShake(1);
                //    break;
                //case 6: /* shake off */
                //    setShake(0);
                //    break;
                //case 8: /* room scale? */
                //    a = getVarOrDirectByte(0x80);
                //    b = getVarOrDirectByte(0x40);
                //    c = getVarOrDirectByte(0x20);
                //    unkRoomFunc2(b, c, a, a, a);
                //    break;
                //case 9: /* ? */
                //    _saveLoadFlag = getVarOrDirectByte(0x80);
                //    _saveLoadData = getVarOrDirectByte(0x40);
                //    _saveLoadData = 0; /* TODO: weird behaviour */
                //    break;
                //case 11: /* ? */
                //    a = getVarOrDirectWord(0x80);
                //    b = getVarOrDirectWord(0x40);
                //    c = getVarOrDirectWord(0x20);
                //    _opcode = fetchScriptByte();
                //    d = getVarOrDirectByte(0x80);
                //    e = getVarOrDirectByte(0x40);
                //    unkRoomFunc2(d, e, a, b, c);
                //    break;
                //case 12: /* ? */
                //    a = getVarOrDirectWord(0x80);
                //    b = getVarOrDirectWord(0x40);
                //    c = getVarOrDirectWord(0x20);
                //    _opcode = fetchScriptByte();
                //    d = getVarOrDirectByte(0x80);
                //    e = getVarOrDirectByte(0x40);
                //    unkRoomFunc3(d, e, a, b, c);
                //    break;
                //
                //case 13: /* ? */
                //    error("roomops:13 not implemented");
                //    break;
                //case 14: /* ? */
                //    error("roomops:14 not implemented");
                //    break;
                //case 15: /* palmanip? */
                //    a = getVarOrDirectByte(0x80);
                //    _opcode = fetchScriptByte();
                //    b = getVarOrDirectByte(0x80);
                //    c = getVarOrDirectByte(0x40);
                //    _opcode = fetchScriptByte();
                //    d = getVarOrDirectByte(0x80);
                //    unkRoomFunc4(b, c, a, d, 1);
                //    break;
                //
                //case 16: /* ? */
                //    a = getVarOrDirectByte(0x80);
                //    b = getVarOrDirectByte(0x40);
                //    if (b!=0)
                //        _colorCycleDelays[a] = 0x4000 / (b*0x4C);
                //    else
                //        _colorCycleDelays[a] = 0;
                //    break;
            }

        }

        private void OpVerbCommand()
        {
            Byte verbId = GetVarOrDirectByte(0x80, currentOpCode);
            Verb verb = null;
            
            var subOpCode = DataReader.ReadByte();

            while (subOpCode != 0xFF)
            {
                switch (subOpCode)
                {
                    // load image
                    case 1:
                        Int16 a = GetVarOrDirectWord(0x80, subOpCode);
                        //if (verb)
                        //{
                        //    setVerbObject(_roomResource, a, verb);
                        //    vs->type = 1;
                        //}
                        break;
                    // load from code
                    case 2:
                        verb = resourceManager.Load<Verb>("VERB", verbId, DataReader);
                        //loadPtrToResource(8, slot, NULL);
                        //if (slot == 0)
                        //    nukeResource(8, slot);
                        //vs->type = 0;
                        //vs->imgindex = 0;
                        break;
                    // color
                    case 3:
                        GetVarOrDirectByte(0x80, subOpCode);
                        //vs->color = 
                        break;
                    // hicolor
                    case 4:
                        GetVarOrDirectByte(0x80, subOpCode);
                        //vs->hicolor = getVarOrDirectByte(0x80);
                        break;
                    // set xy
                    case 5:
                    case 197:
                        verb.X = GetVarOrDirectWord(0x80, subOpCode);
                        verb.Y = GetVarOrDirectWord(0x40, subOpCode);
                        break;
                    // set on
                    case 6:
                        //vs->curmode = 1;
                        break;
                    // set off
                    case 7:
                        //vs->curmode = 0;
                        break;
                    // delete
                    case 8:
                        //killVerb(slot);
                        break;
                    // new
                    case 9:
                        verb = resourceManager.FindVerb(verbId);
                        //verb.ResourceId->verbid = verb;
                        //vs->color = 2;
                        //vs->hicolor = 0;
                        //vs->dimcolor = 8;
                        //vs->type = 0;
                        //vs->charset_nr = textslot.charset[0];
                        //vs->curmode = 0;
                        //vs->saveid = 0;
                        //vs->key = 0;
                        //vs->center = 0;
                        //vs->imgindex = 0;
                        break;

                    case 16: /* set dim color */
                        //vs->dimcolor = getVarOrDirectByte(0x80);
                        break;
                    case 17: /* dim */
                        //vs->curmode = 2;
                        break;
                    case 18: /* set key */
                        GetVarOrDirectByte(0x80, subOpCode);
                        //vs->key = 
                        break;
                    case 19: /* set center */
                        //vs->center = 1;
                        break;
                    case 20: /* set to string */
                        GetVarOrDirectWord(0x80, subOpCode);
                        //ptr = getResourceAddress(7, getVarOrDirectWord(0x80));
                        //if (!ptr)
                        //    nukeResource(8, slot);
                        //else
                        //{
                        //    loadPtrToResource(8, slot, ptr);
                        //}
                        //if (slot == 0)
                        //    nukeResource(8, slot);
                        //vs->type = 0;
                        //vs->imgindex = 0;
                        break;
                    case 22: /* assign object */
                    case 150:
                        GetVarOrDirectWord(0x80, subOpCode);
                        GetVarOrDirectByte(0x40, subOpCode);
                        //a = getVarOrDirectWord(0x80);
                        //b = getVarOrDirectByte(0x40);
                        //if (slot && vs->imgindex != a)
                        //{
                        //    setVerbObject(b, a, slot);
                        //    vs->type = 1;
                        //    vs->imgindex = a;
                        //}
                        break;
                    case 23: /* set back color */
                        GetVarOrDirectByte(0x80, subOpCode);
                        //vs->bkcolor = getVarOrDirectByte(0x80);
                        break;
                    default:
                        throw new NotImplementedException("VerbCommand subCode not found");
                }
                subOpCode = DataReader.ReadByte();
            }
            if(verb != null)
                sceneManager.Verbs.Add(verb);
        }

        private void OpCursorCommand()
        {
            var subOpCode = DataReader.ReadByte();
            switch (subOpCode)
            {
                // show cursor
                case 1:
                    scriptManager.WriteVariable((uint)VariableV5.VAR_CURSORSTATE, 1, this);
                    break;
                // hide cursor
                case 2:
                    scriptManager.WriteVariable((uint)VariableV5.VAR_CURSORSTATE, 0, this);
                    break;
                case 3:
                    scriptManager.WriteVariable((uint)VariableV5.VAR_USERPUT, 1, this);
                    break;
                case 4:
                    scriptManager.WriteVariable((uint)VariableV5.VAR_USERPUT, 0, this);
                    break;
                // init charset
                case 13:
                    {
                        byte charsetId = DataReader.ReadByte();
                        break;
                    }
                // unk
                case 14:
                    {
                        Int16[] charsetId = GetWordVararg();
                        //initCharset(getVarOrDirectByte(0x80));
                        break;
                    }
                default:
                    throw new NotImplementedException("CursorCommand subCode not found");
            };
        }

        private void OpExpression()
        {
            uint resultId = DataReader.ReadUInt16();
            byte operation = DataReader.ReadByte();
            string logOperation = string.Empty;

            while (operation != 0xFF)
            {
                int a, b;

                switch (operation)
                {
                    // Push value
                    case 1:
                        a = DataReader.ReadUInt16();
                        scriptManager.Push(a);
                        logOperation = string.Format("{0}", a);
                        break;
                    // Add
                    case 2:
                        a = scriptManager.Pop();
                        b = scriptManager.Pop();
                        scriptManager.Push(a + b);
                        logOperation = string.Format("{0} + {1}", a, b);
                        break;
                    // Sub
                    case 3:
                        a = scriptManager.Pop();
                        b = scriptManager.Pop();
                        scriptManager.Push(b - a);
                        logOperation = string.Format("{0} - {1}", b, a);
                        break;
                    // Mul
                    case 4:
                        a = scriptManager.Pop();
                        b = scriptManager.Pop();
                        scriptManager.Push(a * b);
                        logOperation = string.Format("{0} * {1}", a, b);
                        break;
                    // Div
                    case 5:
                        a = scriptManager.Pop();
                        b = scriptManager.Pop();
                        if (a == 0)
                            throw new DivideByZeroException();
                        scriptManager.Push(b / a);
                        logOperation = string.Format("{0} / {1}", b, a);
                        break;
                    // Other operation
                    case 6:
                        {
                            //ExecuteInstruction();
                            currentOpCode = DataReader.ReadByte();
                            opCodeHandlers[currentOpCode]();

                            var value = scriptManager.ReadVariable(0, this);
                            scriptManager.Push(Convert.ToInt32(value));
                            logOperation = string.Format("{0}", value);
                            break;
                        }

                    // Push variable
                    case 129:
                        {
                            var value = scriptManager.ReadVariable(GetVariableAddress(), this);
                            scriptManager.Push(Convert.ToInt32(value));
                            logOperation = string.Format("{0}", value);
                        }
                        break;
                }
                operation = DataReader.ReadByte();
            }
            int finalValue = scriptManager.Pop();
            scriptManager.WriteVariable(resultId, finalValue, this);
            this.LogVariable(resultId, " = " + logOperation);
        }

        private void OpStringCommand()
        {
            var subOpCode = DataReader.ReadByte();
            switch (subOpCode)
            {
                // load string
                // TODO: Store the string in the correct slot in a ScriptEngine variable or in the resource manager?
                case 1:
                    {
                        var id = DataReader.ReadByte();
                        var scummString = resourceManager.Load<ScummString>("STRN", id, DataReader);
                        #if !COMPARE
                        this.LogOpCodeInformations("LoadString(\"{0}\")", scummString.Stream);
                        #endif
                        break;
                    }
                case 2:
                    break;
                // set string char - not implemented
                case 3:
                    var a = DataReader.ReadByte();
                    var b = DataReader.ReadByte();
                    var c = DataReader.ReadByte();
                    break;
                case 4:
                    break;
                // create empty string - not implemented
                case 5:
                    {
                        var id = DataReader.ReadByte();
                        var size = DataReader.ReadByte();
                        break;
                    }
            };
        }

        private void OpResourceCommand()
        {
            var subOpCode = DataReader.ReadByte();

            byte resourceId = 0;

            if (subOpCode != 17)
                resourceId = DataReader.ReadByte();

            switch (subOpCode)
            {
                // load script
                case 1:
                    resourceManager.Load<Script>("SCRP", resourceId);
                    #if !COMPARE
                    this.LogOpCodeInformations("LoadScript({0})", resourceId);
                    #endif
                    break;

                // load room
                case 4:
                    resourceManager.Load<Room>("ROOM", resourceId);
                    #if !COMPARE
                    this.LogOpCodeInformations("LoadRoom({0})", resourceId);
                    #endif
                    break;

                // lock script
                case 9:
                    break;

                // load charset
                case 18:
                    resourceManager.Load<Charset>("CHRS", resourceId);
                    #if !COMPARE
                    this.LogOpCodeInformations("LoadCharSet({0})", resourceId);
                    #endif
                    break;
            }
            //case 2: /* load sound */
            //	ensureResourceLoaded(4, res);
            //	break;
            //case 3: /* load costume */
            //	ensureResourceLoaded(3, res);
            //	break;
            //case 4: /* load room */
            //	ensureResourceLoaded(1, res);
            //	break;
            //case 5: /* nuke script */
            //	setResourceFlags(2, res, 0x7F);
            //	break;
            //case 6: /* nuke sound */
            //	setResourceFlags(4, res, 0x7F);
            //	break;
            //case 7: /* nuke costume */
            //	setResourceFlags(3, res, 0x7F);
            //	break;
            //case 8: /* nuke room */
            //	setResourceFlags(1, res, 0x7F);
            //	break;

            //case 10:/* lock sound */
            //	lock(4,res);
            //	break;
            //case 11:/* lock costume */
            //	lock(3,res);
            //	break;
            //case 12:/* lock room */
            //	if (res > 0x7F)
            //		res = _resourceMapper[res&0x7F];
            //	lock(1,res);
            //	break;
            //case 13:/* unlock script */
            //	if (res >= _numGlobalScriptsUsed)
            //		break;
            //	unlock(2,res);
            //	break;
            //case 14:/* unlock sound */
            //	unlock(4,res);
            //	break;
            //case 15:/* unlock costume */
            //	unlock(3,res);
            //	break;
            //case 16:/* unlock room */
            //	if (res > 0x7F)
            //		res = _resourceMapper[res&0x7F];
            //	unlock(1,res);
            //	break;
            //case 17:/* clear heap */
            //	heapClear(0);
            //	unkHeapProc2(0,0);
            //	break;
            //
            //case 19:/* nuke charset */
            //	nukeCharset(res);
            //	break;
            //case 20:/* ? */
            //	unkResProc(getVarOrDirectWord(0x40), res);
            //	break;
            //}
        }

        #endregion

        #region Logging
        private void LogOpCodeInformations(string message, params object[] arguments)
        {
            #if DEBUG
                #if COMPARE
                    this.logFile.WriteLine(string.Format(message, arguments));
                #else
                    this.logFile.WriteLine("Script Engine\t[{0:X4}] ({1:X2}) {2}", this.currentInstructionOffset, this.currentOpCode, string.Format(message, arguments));
                #endif
            #endif
        }

        private void LogVariable(uint variableAddress, string message, params object[] arguments)
        {
            #if DEBUG
                #if !COMPARE
                    var variableName = "VAR_" + variableAddress;

                    if (Enum.IsDefined(typeof(VariableV5), variableAddress))
                    {
                        variableName = ((VariableV5)variableAddress).ToString();
                    }

                    this.logFile.WriteLine("Script Engine\t[{0:X4}] ({1:X2}) {2}{3}", this.currentInstructionOffset, this.currentOpCode, variableName, string.Format(message, arguments));
                #endif
            #endif
        }
        #endregion
    }
}
