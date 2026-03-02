using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace KickstartService
{
    public static class AutoPatcher
    {
        private const byte From = 0x7F;
        private const byte To = 0x6F;

        /*
            68k Instruction Sequence (Big Endian):

            E0 80        ASR.L   #8,D0
            72 7F        MOVEQ   #$7F,D1   <-- Patch this byte (7F -> 6F)
            C0 81        MULU    D1,D0

            Patch position = Pattern offset + 3
        */
        private static readonly byte[] Pattern =
            { 0xE0, 0x80, 0x72, 0x7F, 0xC0, 0x81 };

        public static bool TryPatch(byte[] rom, out int patchedOffset)
        {
            patchedOffset = -1;

            if (rom == null || rom.Length != 512 * 1024)
                return false;

            var hits = FindAll(rom, Pattern);

            if (hits.Count == 0)
                return false;

            int hit = hits.Min();        // niedrigster Offset
            int patchOffset = hit + 3;   // Position des 0x7F

            if (rom[patchOffset] != From)
                return false;            // Sicherheitsprüfung

            rom[patchOffset] = To;
            patchedOffset = patchOffset;
            return true;
        }

        private static List<int> FindAll(byte[] data, byte[] pattern)
        {
            var results = new List<int>();

            for (int i = 0; i <= data.Length - pattern.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (data[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                    results.Add(i);
            }

            return results;
        }
    }

}
