using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.Resources.Graphics;
namespace Scumm.Engine.Resources.Scripts
{
    public class ScriptV5 : Script
    {
        private Action[] opCodeHandlers;

        public ScriptV5(string resourceId, byte[] data, ScriptManager scriptMngr, ResourceManager resourceMngr, SceneManager sceneMngr)
            : base(resourceId, data, scriptMngr, resourceMngr, sceneMngr)
        {
            opCodeHandlers = new Action[256];

            opCodeHandlers[8] = new Action(OpIsNotEqual);
            opCodeHandlers[10] = new Action(OpRunScript);
            opCodeHandlers[12] = new Action(OpResourceCommand);
            opCodeHandlers[16] = new Action(OpGetObjectOwner);
            opCodeHandlers[19] = new Action(OpActorCommand);
            opCodeHandlers[20] = new Action(OpPrint);
            opCodeHandlers[24] = new Action(OpJumpRelative);
            opCodeHandlers[26] = new Action(OpMove);
            opCodeHandlers[37] = new Action(OpPickupObject);
            opCodeHandlers[38] = new Action(OpSetVariableRange);
            opCodeHandlers[39] = new Action(OpStringCommand);
            opCodeHandlers[40] = new Action(OpEqualZero);
            opCodeHandlers[41] = new Action(OpSetOwnerOf);
            opCodeHandlers[42] = new Action(OpRunScript);
            opCodeHandlers[44] = new Action(OpCursorCommand);
            opCodeHandlers[51] = new Action(OpRoomCommand);
            opCodeHandlers[55] = new Action(OpStartObject);
            opCodeHandlers[68] = new Action(OpIsLess);
            opCodeHandlers[70] = new Action(OpIncrement);
            opCodeHandlers[72] = new Action(OpIsEqual);
            opCodeHandlers[90] = new Action(OpAdd);
            opCodeHandlers[104] = new Action(OpGetScriptRunning);
            opCodeHandlers[114] = new Action(OpLoadRoom);
            opCodeHandlers[120] = new Action(OpIsGreater);
            opCodeHandlers[122] = new Action(OpVerbCommand);
            opCodeHandlers[128] = new Action(OpBreakHere);
            opCodeHandlers[129] = new Action(OpPutActor);
            opCodeHandlers[136] = new Action(OpIsNotEqual);
            opCodeHandlers[139] = new Action(OpGetVerbEntryPoint);
            opCodeHandlers[145] = new Action(OpAnimateActor);
            opCodeHandlers[154] = new Action(OpMove);
            opCodeHandlers[160] = new Action(OpStopObjectCode);
            opCodeHandlers[168] = new Action(OpNotEqualZero);
            opCodeHandlers[172] = new Action(OpExpression);
            opCodeHandlers[173] = new Action(OpPutActorInRoom);
            opCodeHandlers[177] = new Action(OpGetInventoryCount);
            opCodeHandlers[183] = new Action(OpStartObject);
            opCodeHandlers[184] = new Action(OpIsLessOrEqual);
            opCodeHandlers[196] = new Action(OpIsLess);
            opCodeHandlers[204] = new Action(OpPseudoRoom);
            opCodeHandlers[210] = new Action(OpActorFollowCamera);
            opCodeHandlers[213] = new Action(OpActorFromPos);
            opCodeHandlers[232] = new Action(OpGetScriptRunning);
            opCodeHandlers[245] = new Action(OpFindObject);
            opCodeHandlers[248] = new Action(OpIsGreater);
            opCodeHandlers[250] = new Action(OpVerbCommand);
            opCodeHandlers[253] = new Action(OpFindInventory);
        }

        public override void ExecuteInstruction()
        {
            currentOpCode = DataReader.ReadByte();

            if (opCodeHandlers[currentOpCode] != null)
            {
                if (currentOpCode != 26 && currentOpCode != 39)
                {
                    //Console.WriteLine("{0} : {1}", ResourceId, currentOpCode);
                    //ScummEngine.Instance.LogFile.WriteLine("{0} : {1}", ResourceId, currentOpCode);
                    //ScummEngine.Instance.LogFile.WriteLine("{0}", currentOpCode);
                }
                opCodeHandlers[currentOpCode]();
                
            }
            else if (currentOpCode != 0xFF)
            {
                Console.Write("Unknown opcode {0}", currentOpCode);
            }
        }

        private void StartScene(byte roomId)
        {
            scriptManager.WriteVariable((uint)VariableV5.VAR_NEW_ROOM, roomId, this);
            RunExitScript(scriptManager.CurrentRoomId);

            scriptManager.WriteVariable((uint)VariableV5.VAR_ROOM, roomId, this);
            scriptManager.WriteVariable((uint)VariableV5.VAR_ROOM_RESOURCE, roomId, this);

            scriptManager.CurrentRoomId = roomId;

            if (roomId != 0)
            {
                sceneManager.CurrentRoom = resourceManager.Load<Room>("ROOM", roomId);
                RunEntryScript(roomId);
            }
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

        private void RunHook(byte hookId)
        {
            Byte scriptId = Convert.ToByte(scriptManager.ReadVariable((uint)VariableV5.VAR_HOOK_SCRIPT, this));
            if (scriptId != 0)
            {
                Script hook = resourceManager.Load<Script>("SCRP", scriptId);

                int[] localVars = new int[1];
                localVars[0] = hookId;
                hook.Run(localVars);
            }
        }

        private void DecodeParseString(int actor)
        {
            int textSlot;

            switch (actor)
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
                        GetVarOrDirectWord(0x80);
                        GetVarOrDirectWord(0x40);
                        //_stringXpos[textSlot] = getVarOrDirectWord(0x80);
                        //_stringYpos[textSlot] = getVarOrDirectWord(0x40);
                        //_stringOverhead[textSlot] = 0;
                        break;
                    case 1: /* color */
                        GetVarOrDirectByte(0x80);
                        //_stringColor[textSlot] = 
                        break;
                    case 2: /* right */
                        GetVarOrDirectByte(0x80);
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
                        GetVarOrDirectWord(0x80);
                        GetVarOrDirectWord(0x40);
                        break;
                    case 15:
                        //_messagePtr = _scriptPointer;
                        switch (textSlot)
                        {
                            //case 0: actorTalk(); break;
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

        private void OpMove()
        {
            uint variableAddress = GetVariableAddress();
            Int16 variableValue = GetVarOrDirectWord(0x80);

            scriptManager.WriteVariable(variableAddress, variableValue, this);
        }

        private void OpPrint()
        {
            byte actor = GetVarOrDirectByte(0x80);
            DecodeParseString(actor);
        }

        private void OpStartObject()
        {
            var obj = GetVarOrDirectWord(0x80);
            var script = GetVarOrDirectByte(0x40);
            short[] data = GetWordVararg();
        }

        private void OpSetOwnerOf()
        {
            var objId = GetVarOrDirectWord(0x80);
            var owner = GetVarOrDirectByte(0x40);

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
            var owner = GetVarOrDirectByte(0x80);

            int count = 0;
            for (int i = 0; i < sceneManager.InventorySize; ++i)
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
            var owner = GetVarOrDirectByte(0x80);
            var b = GetVarOrDirectByte(0x40);

            int count = 1;
            for (int i = 0; i < sceneManager.InventorySize; ++i)
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
            var a = GetVarOrDirectWord(0x80);
            var b = GetVarOrDirectWord(0x40);

            scriptManager.WriteVariable(address, 50, this);
        }

        private void OpGetObjectOwner()
        {
            uint variableAddress = GetVariableAddress();
            Int16 objectId = GetVarOrDirectWord(0x80);
            Object obj = resourceManager.FindObject(objectId);

            scriptManager.WriteVariable(variableAddress, obj.OwnerState, this);
        }

        private void OpRunScript()
        {
            byte scriptId = GetVarOrDirectByte(0x80);
            Int16[] data = GetWordVararg();

            int a, b;
            // TODO: I don't know what these are used for
            a = b = 0;
            if ((currentOpCode & 0x20) != 0) a = 1;
            if ((currentOpCode & 0x40) != 0) b = 1;

            if (scriptId < 200)
            {
                Script script = resourceManager.Load<Script>("SCRP", scriptId);
                script.Run();
            }
        }

        private void OpGetScriptRunning()
        {
            uint address = GetVariableAddress();
            Byte script = GetVarOrDirectByte(0x80);

            scriptManager.WriteVariable(address, 1, this);
        }

        private void OpBreakHere()
        {
            Stop();
        }

        private void OpStopObjectCode()
        {
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
	        Int16 objX = GetVarOrDirectWord(0x80);
            Int16 objY = GetVarOrDirectWord(0x40);
	        //setResult(findObject(t, getVarOrDirectWord(0x40)));
        }

        private void OpActorFollowCamera()
        {
	        //int a = camera._follows;
            //
            Actor actor = resourceManager.FindActor(GetVarOrDirectByte(0x80));
            if (actor.RoomID != scriptManager.CurrentRoomId)
                StartScene(actor.RoomID);
            //, "actorFollowCamera"));
            //
	        //if (camera._follows != a) 
		    //    runHook(0);
            //
	        //camera._movingToActor = 0;
        }

        private void OpActorFromPos() 
        {
	        uint address = GetVariableAddress();
            Int16 objX = GetVarOrDirectWord(0x80);
            Int16 objY = GetVarOrDirectWord(0x40);
            //setResult(getActorFromPos(x,y));
        }

        private void OpPutActorInRoom() 
        {
            Actor actor = resourceManager.FindActor(GetVarOrDirectByte(0x80));
	        actor.RoomID = GetVarOrDirectByte(0x40);
	    }

        private void OpPutActor() 
        {
            GetVarOrDirectByte(0x80);
            GetVarOrDirectWord(0x40);
            GetVarOrDirectWord(0x20);
        }

        private void OpAnimateActor()
        {
            GetVarOrDirectByte(0x80);
            GetVarOrDirectByte(0x40);
        }

        private void OpLoadRoom()
        {
            byte room = GetVarOrDirectByte(0x80);
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
            DataReader.BaseStream.Position += jump;
        }

        private void OpIsNotEqual()
        {
            Int16 a = Convert.ToInt16(scriptManager.ReadVariable(GetVariableAddress(), this));
            Int16 b = GetVarOrDirectWord(0x80);

            Int16 jump = DataReader.ReadInt16();
            if (a == b) DataReader.BaseStream.Position += jump;
        }         

        private void OpIsEqual()
        {
            Int16 a = Convert.ToInt16(scriptManager.ReadVariable(GetVariableAddress(), this));
            Int16 b = GetVarOrDirectWord(0x80);

            Int16 jump = DataReader.ReadInt16();
            if (a != b) DataReader.BaseStream.Position += jump;
        }

        private void OpIsLess()
        {
            Int16 a = Convert.ToInt16(scriptManager.ReadVariable(GetVariableAddress(), this));
            Int16 b = GetVarOrDirectWord(0x80);

            Int16 jump = DataReader.ReadInt16();
            if (b >= a) DataReader.BaseStream.Position += jump;
        }

        private void OpIsGreater()
        {
            Int16 a = Convert.ToInt16(scriptManager.ReadVariable(GetVariableAddress(), this));
            Int16 b = GetVarOrDirectWord(0x80);

            Int16 jump = DataReader.ReadInt16();
            if (b <= a) DataReader.BaseStream.Position += jump;
        }

        private void OpIsLessOrEqual()
        {
            Int16 a = Convert.ToInt16(scriptManager.ReadVariable(GetVariableAddress(), this));
            Int16 b = GetVarOrDirectWord(0x80);

            Int16 jump = DataReader.ReadInt16();
            if (b > a) DataReader.BaseStream.Position += jump;
        }

        private void OpNotEqualZero()
        {
            Int16 value = Convert.ToInt16(scriptManager.ReadVariable(GetVariableAddress(), this));
            Int16 jump = DataReader.ReadInt16();
            if (value == 0) DataReader.BaseStream.Position += jump;
        }

        private void OpEqualZero()
        {
            Int16 value = Convert.ToInt16(scriptManager.ReadVariable(GetVariableAddress(), this));
            Int16 jump = DataReader.ReadInt16();
            if (value != 0) DataReader.BaseStream.Position += jump;
        }

        private void OpAdd()
        {
            uint address = GetVariableAddress();
            Int16 a = GetVarOrDirectWord(0x80);
            scriptManager.WriteVariable(address, Convert.ToInt16(scriptManager.ReadVariable(address, this)) + a, this);
        }

        private void OpIncrement()
        {
            uint address = GetVariableAddress();
            scriptManager.WriteVariable(address, Convert.ToInt16(scriptManager.ReadVariable(address, this)) + 1, this);
        }

        private void OpActorCommand()
        {
            Byte actorId = GetVarOrDirectByte(0x80);
            Actor actor = resourceManager.FindActor(actorId);

            currentOpCode = DataReader.ReadByte();
            while (currentOpCode != 0xFF)
            {
                switch (currentOpCode)
                {
                    // costume
                    case 1:
                        Byte costume = GetVarOrDirectByte(0x80);
                        //setActorCostume(a, );
                        break;
                    case 2: /* walkspeed */
                        GetVarOrDirectByte(0x80);
                        GetVarOrDirectByte(0x40);
                        //i = 
                        //j = 
                        //setActorWalkSpeed(a, i, j);
                        break;
                    case 3: /* sound */
                        GetVarOrDirectByte(0x80);
                        //a->sound = getVarOrDirectByte(0x80);
                        break;
                    case 4: /* walkanim */
                        GetVarOrDirectByte(0x80);
                        //a->walkFrame = getVarOrDirectByte(0x80);
                        break;
                    case 5: /* talkanim */
                        GetVarOrDirectByte(0x80);
                        GetVarOrDirectByte(0x40);
                        //a->talkFrame1 = getVarOrDirectByte(0x80);
                        //a->talkFrame2 = getVarOrDirectByte(0x40);
                        break;
                    case 6: /* standanim */
                        GetVarOrDirectByte(0x80);
                        //a->standFrame = getVarOrDirectByte(0x80);
                        break;
                    case 7: /* ignore */
                        GetVarOrDirectByte(0x80);
                        GetVarOrDirectByte(0x40);
                        GetVarOrDirectByte(0x20);
                        break;
                    // Init
                    case 8: 
                        actor.Init(0);
                        break;
                    case 9: /* elevation */
                        GetVarOrDirectWord(0x80);
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
                        GetVarOrDirectByte(0x80);
                        GetVarOrDirectByte(0x40);
                        //checkRange(32, 0, i, "Illegal palet slot %d");
                        //a->palette[i] = j;
                        //a->needRedraw = 1;
                        break;
                    case 12: /* talk color */
                        //a->talkColor = getVarOrDirectByte(0x80);
                        GetVarOrDirectByte(0x80);
                        break;
                    case 13: /* name */
                        resourceManager.Load<ScummString>("STRN", 0, DataReader);
                        break;
                    case 14: /* initanim */
                        GetVarOrDirectByte(0x80);
                        //a->initFrame = 
                        break;
                    case 16: /* width */
                        GetVarOrDirectByte(0x80);
                        //a->width = 
                        break;
                    case 17: /* scale */
                        GetVarOrDirectByte(0x80);
                        GetVarOrDirectByte(0x40);
                        //a->scalex = 
                        //a->scaley = 
                        break;
                    case 18: /* neverzclip */
                        //a->neverZClip = 0;
                        break;
                    case 19: /* setzclip */
                        //a->neverZClip = 
                        GetVarOrDirectByte(0x80);
                        break;
                    case 20: /* ignoreboxes */
                        //a->ignoreBoxes = 1;
                        //a->neverZClip = 0;
                        //FixRoom:
                        //if (a->room == _currentRoom)
                        //  putActor(a, a->x, a->y, a->room);
                        break;
                    case 21: /* followboxes */
                    //a->ignoreBoxes = 0;
                    //a->neverZClip = 0;
                    //goto FixRoom;

                    case 22: /* animspeed */
                        //a->animSpeed = 
                        GetVarOrDirectByte(0x80);
                        break;
                    case 23: /* unk2 */
                        //a->data8 = 
                        GetVarOrDirectByte(0x80); /* unused */
                        break;
                    default:
                        throw new NotImplementedException("ActorCommand subCode not found");
                }
                currentOpCode = DataReader.ReadByte();
            }
        }

        private void OpRoomCommand()
        {
            currentOpCode = DataReader.ReadByte();

            Int16 a, b, c, d, e;
            switch (currentOpCode)
            {
                // Room scroll
                case 1:
                    a = GetVarOrDirectWord(0x80);
                    b = GetVarOrDirectWord(0x40);
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
                    break;
                // Set screen
                case 3:
                    a = GetVarOrDirectWord(0x80);
                    b = GetVarOrDirectWord(0x40);
                    //initScreens(0,a,320,b);
                    break;
                // Set palette color
                case 4:
                case 228:
                    a = GetVarOrDirectWord(0x80);
                    b = GetVarOrDirectWord(0x40);
                    c = GetVarOrDirectWord(0x20);
                    this.currentOpCode = DataReader.ReadByte();
                    d = GetVarOrDirectByte(0x80);
                    //setPalColor(d, a, b, c); /* index, r, g, b */
                    break;
                case 10: // ?
                    a = GetVarOrDirectWord(0x80);
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
            Byte verb = GetVarOrDirectByte(0x80);

            currentOpCode = DataReader.ReadByte();
            while (currentOpCode != 0xFF)
            {
                switch (currentOpCode)
                {
                    // load image
                    case 1:
                        Int16 a = GetVarOrDirectWord(0x80);
                        //if (verb)
                        //{
                        //    setVerbObject(_roomResource, a, verb);
                        //    vs->type = 1;
                        //}
                        break;
                    // load from code
                    case 2:
                        resourceManager.Load<Verb>("VERB", verb, DataReader);
                        //loadPtrToResource(8, slot, NULL);
                        //if (slot == 0)
                        //    nukeResource(8, slot);
                        //vs->type = 0;
                        //vs->imgindex = 0;
                        break;
                    // color
                    case 3:
                        GetVarOrDirectByte(0x80);
                        //vs->color = 
                        break;
                    // hicolor
                    case 4:
                        GetVarOrDirectByte(0x80);
                        //vs->hicolor = getVarOrDirectByte(0x80);
                        break;
                    // set xy
                    case 5:
                    case 197:
                        GetVarOrDirectWord(0x80);
                        GetVarOrDirectWord(0x40);
                        //vs->x = getVarOrDirectWord(0x80);
                        //vs->y = getVarOrDirectWord(0x40);
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
                    //slot = getVerbSlot(verb, 0);
                    //if (slot == 0)
                    //{
                    //    for (slot = 1; slot < _maxVerbs; slot++)
                    //    {
                    //        if (verbs[slot].verbid == 0)
                    //            break;
                    //    }
                    //    if (slot == _maxVerbs)
                    //        error("Too many verbs");
                    //}
                    //vs = &verbs[slot];
                    //vs->verbid = verb;
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
                        GetVarOrDirectByte(0x80);
                        //vs->key = 
                        break;
                    case 19: /* set center */
                        //vs->center = 1;
                        break;
                    case 20: /* set to string */
                        GetVarOrDirectWord(0x80);
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
                        GetVarOrDirectWord(0x80);
                        GetVarOrDirectByte(0x40);
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
                        GetVarOrDirectByte(0x80);
                        //vs->bkcolor = getVarOrDirectByte(0x80);
                        break;
                    default:
                        throw new NotImplementedException("VerbCommand subCode not found");
                }
                currentOpCode = DataReader.ReadByte();
            }

            //drawVerb(slot, 0);
            //verbMouseOver(0);
        }

        private void OpCursorCommand()
        {
            currentOpCode = DataReader.ReadByte();
            switch (currentOpCode)
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
            while (operation != 0xFF)
            {
                int a, b;

                switch (operation)
                {
                    // Push value
                    case 1:
                        scriptManager.Push(DataReader.ReadUInt16());
                        break;
                    // Add
                    case 2:
                        a = scriptManager.Pop();
                        b = scriptManager.Pop();
                        scriptManager.Push(a + b);
                        break;
                    // Sub
                    case 3:
                        a = scriptManager.Pop();
                        b = scriptManager.Pop();
                        scriptManager.Push(b - a);
                        break;
                    // Mul
                    case 4:
                        a = scriptManager.Pop();
                        b = scriptManager.Pop();
                        scriptManager.Push(a * b);
                        break;
                    // Div
                    case 5:
                        a = scriptManager.Pop();
                        b = scriptManager.Pop();
                        if (a == 0)
                            throw new DivideByZeroException();
                        scriptManager.Push(b / a);
                        break;
                    // Other operation
                    case 6:
                        {
                            ExecuteInstruction();
                            var value = scriptManager.ReadVariable(0, this);
                            scriptManager.Push(Convert.ToInt32(value));
                            break;
                        }

                    // Push variable
                    case 129:
                        {
                            var value = scriptManager.ReadVariable(GetVariableAddress(), this);
                            scriptManager.Push(Convert.ToInt32(value));
                        }
                        break;
                }
                operation = DataReader.ReadByte();
            }
            int finalValue = scriptManager.Pop();
            scriptManager.WriteVariable(resultId, finalValue, this);
        }

        private void OpStringCommand()
        {
            currentOpCode = DataReader.ReadByte();
            switch (currentOpCode)
            {
                // load string
                case 1:
                    {
                        var id = DataReader.ReadByte();
                        resourceManager.Load<ScummString>("STRN", id, DataReader);
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
            currentOpCode = DataReader.ReadByte();

            byte resourceId = 0;
            if (currentOpCode != 17)
                resourceId = DataReader.ReadByte();

            switch (currentOpCode)
            {
                // load script
                case 1:
                    resourceManager.Load<Script>("SCRP", resourceId);
                    break;

                // load room
                case 4:
                    resourceManager.Load<Room>("ROOM", resourceId);
                    break;

                // lock script
                case 9:
                    break;

                // load charset
                case 18:
                    resourceManager.Load<Charset>("CHRS", resourceId);
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
    }
}
