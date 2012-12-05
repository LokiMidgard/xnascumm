using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scumm.Engine.Resources.Scripts
{
    [Flags]
    public enum ScriptStatus
    {
        Running,
        Stopped,
        Paused,
        Frozen
    }
}
