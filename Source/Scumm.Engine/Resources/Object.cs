using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scumm.Engine.Resources.Scripts;

namespace Scumm.Engine.Resources
{
    public class Object : Resource
    {
        Byte ownerState;
        UInt32 classData;
        UInt16 id;

        Script[] verbs;
        Image image;

        public Object()
            : base("")
        {
        }

        public UInt16 Id
        {
            get { return id; }
            set { id = value; }
        }
        public Byte OwnerState
        {
            get { return ownerState; }
            set { ownerState = value; }
        }
        public UInt32 ClassData
        {
            get { return classData; }
            set { classData = value; }
        }

        public void setOwnerState(UInt16 value)
        {
            ownerState = (Byte)(value << 4 | value & 0x0F);
        }

        public Byte getOwnerState()
        {
            return (Byte)(ownerState & 0x0F);
        }
    }
}
