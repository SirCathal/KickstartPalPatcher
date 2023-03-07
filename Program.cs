using PalPatcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PalPatcher
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: PalPatcher [Kick1.rom] [Kick2.rom] ...");
                Console.WriteLine("That will patch the given Files only");
                Console.WriteLine("");
                Console.WriteLine("Usage  PalPatcher [KickRomFolder]");
                Console.WriteLine("That will patch all supportet Kickstart Files in the given folder");
                Console.WriteLine("");
                Console.WriteLine("Important: No original File will be changed!");
                Console.WriteLine("Any supportet Kickstart will be copied and a '.Pal.Rom' suffix will be added");
                Console.WriteLine("");
                Console.WriteLine("Please keep in Mind that the patchet Kickstarts will only work with a 8372A and 8375 Agnus Chips");
                Console.WriteLine("which are switchable from NTSC to PAL!");
                Console.WriteLine("");
                Console.WriteLine("Attention! The program needs the .ROM executable files.");
                Console.WriteLine("The .bin 'ByteSwap' versions for eprom burning are not recognized!");
                Console.WriteLine("If the new ROM file created by the program is to be burned to an Eprom, a ByteSwap must be made before.");
                Console.WriteLine("");
                Console.WriteLine("Supported in this Version:");
                foreach (var patch in GetPatches().OrderBy(n => n.Name))
                {
                    Console.WriteLine($"{patch.Name} {patch.Version} original Checksum: ${patch.Checksum.ToString("X")}");
                }
                Console.WriteLine("");
                Console.WriteLine("Spechial Thanks to the A1K.org Users A10001986 and DingensCGN for the Patch Data");
            }

            var files = new List<string>();
            foreach (string arg in args)
            {

                if (Directory.Exists(arg))
                {
                    foreach (var f in Directory.GetFiles(arg))
                    {
                        var info = new FileInfo(f);
                        if (info.Length == 1024 * 1024 || info.Length == 512 * 1024 || info.Length == 256 * 1024)
                        {
                            files.Add(f);
                        }
                    }
                }
                else
                {
                    files.Add(arg);
                }

            }

            var patches = GetPatches();

            foreach (string file in files)
            {
                PatchKickstart(file, patches);
            }
        }

        private static void PatchKickstart(string name, List<Kickstart> patches)
        {
            try
            {
                var BigEndianKickBytes = LoadKickstart(name);
                Console.Write($"Trying File: {name}");
                var csum = CalcChecksumBigEndian(BigEndianKickBytes);
                Console.WriteLine($" With Checksum {csum.ToString("X")}");

                foreach (var patch in patches)
                {
                    if (csum == patch.Checksum)
                    {
                        Console.WriteLine($"Found {patch.Name} {patch.Version} with Checksum {patch.Checksum.ToString("X")}");
                        Console.WriteLine("Apply Pal Patch");
                        if (patch.BytePatchData != null)
                        {
                            foreach (var b in patch.BytePatchData)
                            {
                                BigEndianKickBytes[b.Key] = b.Value;
                            }
                        }

                        if (patch.UIntPatchData != null)
                        {
                            foreach (var b in patch.UIntPatchData)
                            {
                                PokeL(BigEndianKickBytes, b.Key, b.Value);
                            }
                        }

                        var newCheckSum = CalcChecksumBigEndian(BigEndianKickBytes);
                        PokeL(BigEndianKickBytes, BigEndianKickBytes.Length - 24, newCheckSum);
                        var validateCheckSum = CalcChecksumBigEndian(BigEndianKickBytes, true);
                        Console.WriteLine($"New Checksum {newCheckSum.ToString("X")}");

                        if (validateCheckSum == 0xFFFFFFFF)
                        {
                            Console.WriteLine("Validation OK, try to write Patched Kickstart");
                            if (name.ToLower().EndsWith(".rom"))
                            {
                                name = name.Replace(".rom", ".PAL.rom", true, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                name += ".PAL.rom";
                            }

                            Console.Write(name);
                            if (File.Exists(name))
                            {
                                Console.WriteLine(" Error: The output File already exists");
                            }
                            else
                            {
                                File.WriteAllBytes(name, BigEndianKickBytes);
                                Console.WriteLine(" OK.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Validation Failed!");
                        }
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:");
                Console.WriteLine(ex.ToString());
            }
        }

        private static byte[] LoadKickstart(string Filename)
        {
            var kick = File.ReadAllBytes(Filename);
            return kick;
        }

        private static void PokeL(Memory<byte> data, Int32 offset, UInt32 value)
        {
            var address = data.Slice(offset, 4).Span;
            WriteUInt32BigEndian(address, value);
        }

        private static UInt32 CalcChecksumBigEndian(Memory<byte> data, bool onlyValidate = false)
        {
            UInt64 checksum = 0;

            for (var currentByte = 0; currentByte < data.Length; currentByte += 4)
            {
                var big = ReadUInt32BigEndian(data.Slice(currentByte, 4).ToArray()); // 4 Byte als Unsignt 32Bit Int BigEndian Lesen
                if (currentByte != data.Length - 24 && !onlyValidate)
                // Am ende -24 Bytes steth die Checksumme die darf nicht hinzugerechnet werden
                // Zum Validieren wird sie hinzugerechnet und das Ergebniss muss 0xFFFFFFFF sein.
                {
                    checksum += big; // checksumme berechnen
                }
            }

            var ergsum = checksum % UInt32.MaxValue; // Checksumme hat 32Bit daher Modulu 2^32
            return ~Convert.ToUInt32(ergsum); // Checksumme Invertieren
        }

        private static List<Kickstart> GetPatches()
        {
            var erg = new List<Kickstart>()
            {
                new Kickstart()
                {
                    Name ="Kickstart 1.2",
                    Version ="Rev 33.192",
                    Checksum = 0x56F2E2A6,
                    BytePatchData = null,
                    UIntPatchData = new Dictionary<Int32, UInt32>()
                    {
                        {0xB058, 0x700433FC },
                        {0xB05C, 0x002000DF },
                        {0xB060, 0xF1DC4E75 },
                    }
                },
                new Kickstart()
                {
                    Name ="Kickstart 1.3",
                    Version ="Rev 34.5",
                    Checksum = 0x15267DB3,
                    BytePatchData = null,
                    UIntPatchData = new Dictionary<Int32, UInt32>()
                    {
                        {0xB00C, 0x700433FC },
                        {0xB010, 0x002000DF },
                        {0xB014, 0xF1DC4E75 },
                    }
                },
                new Kickstart()
                {
                    Name ="Kickstart 2.04",
                    Version ="Rev 37.175",
                    Checksum = 0x000B927C,
                     BytePatchData = new Dictionary<int, byte>()
                    {
                        {0x285C3,0x6F},
                    },
                    UIntPatchData = null
                },
                new Kickstart()
                {
                    Name ="Kickstart 2.05",
                    Version ="Rev 37.350",
                    Checksum = 0xBA5D5FA4,
                     BytePatchData = new Dictionary<int, byte>()
                    {
                        {0x031E0F,0x6F},
                    },
                    UIntPatchData = null
                },
                new Kickstart()
                {
                    Name ="Kickstart 3.1",
                    Version ="Rev 40.63",
                    Checksum = 0x9FDEEEF6,
                    BytePatchData = new Dictionary<int, byte>()
                    {
                        {0x0BD79,0x6F},
                    },
                    UIntPatchData = null
                },
                new Kickstart()
                {
                    Name ="Kickstart 3.1.4",
                    Version ="Rev 46.143",
                    Checksum = 0xBE662BCF,
                    BytePatchData = new Dictionary<int, byte>()
                    {
                        {0xEF0B,0x6F},
                    },
                    UIntPatchData = null
                },
                new Kickstart()
                {
                    Name ="Kickstart 3.2.0",
                    Version ="Rev 47.69",
                    Checksum = 0x035D98F3,
                    BytePatchData = new Dictionary<int, byte>()
                    {
                        {0xEF8B,0x6F},
                    },
                    UIntPatchData = null
                },
                new Kickstart()
                {
                    Name ="Kickstart 3.2.1",
                    Version ="Rev 47.102",
                    Checksum = 0x4CB8FDD9,
                    BytePatchData = new Dictionary<int, byte>()
                    {
                        {0x0EF93,0x6F},
                    },
                    UIntPatchData = null
                },
                new Kickstart()
                {
                    Name ="Kickstart 3.2.2",
                    Version ="Rev 47.111",
                    Checksum = 0xB1728E0A,
                    BytePatchData = new Dictionary<int, byte>()
                    {
                        {0xEF83,0x6F},
                    },
                    UIntPatchData = null
                },
                new Kickstart()
                {
                    Name ="Kickstart 3.1 A1200",
                    Version ="Rev 40.68",
                    Checksum = 0x87BA7A3E,
                     BytePatchData = new Dictionary<int, byte>()
                    {
                        {0xB48D,0x6F},
                    },
                    UIntPatchData = null
                },
                new Kickstart()
                {
                    Name ="Kickstart 3.2.0 A1200",
                    Version ="Rev 47.96",
                    Checksum = 0xE3B7D1D5,
                     BytePatchData = new Dictionary<int, byte>()
                    {
                        {0xEFF3,0x6F},
                    },
                    UIntPatchData = null
                },
                new Kickstart()
                {
                    Name ="Kickstart 3.2.1 A1200",
                    Version ="Rev 47.102",
                    Checksum = 0x7A47FC4D,
                     BytePatchData = new Dictionary<int, byte>()
                    {
                        {0xEFF7,0x6F},
                    },
                    UIntPatchData = null
                },
                new Kickstart()
                {
                    Name ="Kickstart 3.2.2 A1200",
                    Version ="Rev 47.111",
                    Checksum = 0xDB198F9E,
                     BytePatchData = new Dictionary<int, byte>()
                    {
                        {0xEFE7,0x6F},
                    },
                    UIntPatchData = null
                },
            };
            return erg;
        }
    }
}
