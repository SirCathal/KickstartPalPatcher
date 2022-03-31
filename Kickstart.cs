using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalPatcher
{
    public class Kickstart
    {
        public string? Name { get; set; }

        public string? Version { get; set; }

        public UInt32 Checksum { get; set; }

        public Dictionary<Int32, UInt32>? UIntPatchData { get; set; }
        public Dictionary<Int32, byte>? BytePatchData { get; set; }
    }
}
