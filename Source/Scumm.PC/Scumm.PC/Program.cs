using System;
using Scumm.Engine;

namespace Scumm.PC
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ScummEngine engine = new ScummEngine("Data\\monkey1", "monkey1", 5))
            {
                engine.Run();
            }
        }
    }
#endif
}

