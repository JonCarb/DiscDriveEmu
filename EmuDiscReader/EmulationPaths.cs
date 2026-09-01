using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuDiscReader
{
    public class EmulationPaths
    {
        //Holds the paths for the emulators
        public string? Dolphin { get; set; }
        public string? PCSX2 { get; set; }
        public string? Xemu { get; set; }
        public string? Cemu { get; set; }
        public string? RPCS3 { get; set; }

        //Maybe potentially add more values like cmd parameters, specific folder for caching, etc
    }
}
