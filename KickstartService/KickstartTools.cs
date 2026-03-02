using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
//using System.Runtime.Intrinsics.Arm;
using static System.Buffers.Binary.BinaryPrimitives;

namespace KickstartService
{
    public static class KickstartTools
    {
        public static List<Kickstart> LoadKickstarts(List<string> Files)
        {
            var result = new List<Kickstart>();
            foreach (var file in Files)
            {
                if (File.Exists(file))
                {
                    var kickstart = new Kickstart();
                    kickstart.Rom = LoadKickstart(file);
                    kickstart.PalRom = (byte[])kickstart.Rom.Clone();
                    kickstart.Checksum = CalcChecksumBigEndian(kickstart.Rom);
                    kickstart.Patched = false;
                    kickstart.Name = file;
                    result.Add(kickstart);
                }
            }
            return result;
        }

        public static Kickstart PatchToPal(Kickstart kickstart)
        {
            var patches = GetPatches();
            Dictionary<uint, Kickstart> patchesDict =
                patches
                    .GroupBy(k => k.Checksum)
                    .ToDictionary(g => g.Key, g => g.First());

            if (patchesDict.ContainsKey(kickstart.Checksum))
            {
                kickstart.BytePatchData = patchesDict[kickstart.Checksum].BytePatchData;
                kickstart.UIntPatchData = patchesDict[kickstart.Checksum].UIntPatchData;

                kickstart.Info = "Well Known Kickstart ";

                if (kickstart.BytePatchData != null)
                {
                    foreach (var b in kickstart.BytePatchData)
                    {
                        kickstart.PalRom[b.Key] = b.Value;
                    }
                }

                if (kickstart.UIntPatchData != null)
                {
                    foreach (var b in kickstart.UIntPatchData)
                    {
                        PokeL(kickstart.PalRom, b.Key, b.Value);
                    }
                }
            }
            else
            {
                var success = AutoPatcher.TryPatch(kickstart.PalRom, out int offset);
                if (success)
                {
                    kickstart.Info += "Patch adress found at 0x" + offset.ToString("X") + " ";
                }
                else
                {
                    kickstart.Info += "Sorry, no Patch adress found ";
                }
            }

            kickstart.PalChecksum = CalcChecksumBigEndian(kickstart.PalRom);
            PokeL(kickstart.PalRom, kickstart.PalRom.Length - 24, kickstart.PalChecksum);
            var validateCheckSum = CalcChecksumBigEndian(kickstart.PalRom, true);
            if (validateCheckSum == 0xFFFFFFFF)
            {
                kickstart.Info += "Validation OK ";
                kickstart.Patched = true;
            }
            else
            {
                kickstart.Info += "Validation Failed ";
                kickstart.Patched = false;
            }

            return kickstart;
        }

        //// <summary>
        /// Opens a standard file selection dialog for .rom files.
        /// The user can select one or multiple files.
        /// Returns a list of full file paths (including directory).
        /// If the user cancels the dialog, an empty list is returned.
        /// </summary>
        public static List<string> PickRomFiles(IWin32Window owner = null)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Select ROM file(s)";
                dialog.Filter = "ROM files (*.rom)|*.rom";
                dialog.DefaultExt = "rom";
                dialog.AddExtension = true;
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Multiselect = true;
                dialog.DereferenceLinks = true;
                dialog.RestoreDirectory = true;

                DialogResult result = owner == null
                    ? dialog.ShowDialog()
                    : dialog.ShowDialog(owner);

                if (result != DialogResult.OK || dialog.FileNames.Length == 0)
                    return new List<string>();

                return dialog.FileNames
                             .Where(p => !string.IsNullOrWhiteSpace(p))
                             .Select(p => Path.GetFullPath(p))
                             .Distinct(StringComparer.OrdinalIgnoreCase)
                             .ToList();
            }
        }

        /// <summary>
        /// Opens a standard folder selection dialog.
        /// After selecting a folder, all .rom files inside the folder (top directory only)
        /// are scanned and returned if their size is exactly 256KB or 512KB.
        /// Returns a list of full file paths.
        /// If the user cancels the dialog, an empty list is returned.
        /// </summary>
        public static List<string> PickRomFolderAndScan(IWin32Window owner = null)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select folder containing ROM files";
                dialog.ShowNewFolderButton = false;

                DialogResult result = owner == null
                    ? dialog.ShowDialog()
                    : dialog.ShowDialog(owner);

                if (result != DialogResult.OK || string.IsNullOrWhiteSpace(dialog.SelectedPath))
                    return new List<string>();

                const long size256Kb = 256L * 1024L;
                const long size512Kb = 512L * 1024L;

                return Directory.EnumerateFiles(dialog.SelectedPath, "*.rom", SearchOption.TopDirectoryOnly)
                                .Where(path =>
                                {
                                    try
                                    {
                                        FileInfo fileInfo = new FileInfo(path);
                                        return fileInfo.Exists &&
                                               (fileInfo.Length == size256Kb || fileInfo.Length == size512Kb);
                                    }
                                    catch
                                    {
                                        // Skip files that cannot be accessed
                                        return false;
                                    }
                                })
                                .Select(p => Path.GetFullPath(p))
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .ToList();
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

        public static List<Kickstart> GetPatches()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "patches.json");

            if (!File.Exists(path))
                throw new FileNotFoundException("patches.json not found!", path);

            string json = File.ReadAllText(path);

            var serializer = new JavaScriptSerializer
            {
                MaxJsonLength = int.MaxValue
            };

            var entries = serializer.Deserialize<List<PatchEntry>>(json) ?? new List<PatchEntry>();

            var result = new List<Kickstart>(entries.Count);

            foreach (var e in entries)
            {
                var ks = new Kickstart
                {
                    Name = e.Name,
                    Version = e.Version,
                    Checksum = ParseHexUInt32(e.Checksum),
                    UIntPatchData = e.UIntPatchData != null ? e.UIntPatchData.ToDictionary(
                        kv => ParseHexInt32(kv.Key),
                        kv => ParseHexUInt32(kv.Value)
                    ) : null,
                    BytePatchData = e.BytePatchData != null ? e.BytePatchData.ToDictionary(
                        kv => ParseHexInt32(kv.Key),
                        kv => ParseHexByte(kv.Value)
                    ) : null,
                    Rom = null
                };

                result.Add(ks);
            }

            return result;
        }

        private static string NormalizeHex(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new FormatException("Hex-String ist leer.");

            s = s.Trim();

            // Erlaubt: "0xABCD", "ABCD", "abcd"
            if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                s = s.Substring(2);

            return s;
        }

        private static UInt32 ParseHexUInt32(string s)
        {
            s = NormalizeHex(s);
            return Convert.ToUInt32(s, 16);
        }

        private static Int32 ParseHexInt32(string s)
        {
            s = NormalizeHex(s);
            return Convert.ToInt32(s, 16);
        }

        private static byte ParseHexByte(string s)
        {
            s = NormalizeHex(s);
            return Convert.ToByte(s, 16);
        }
    }

    internal class PatchEntry
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Checksum { get; set; } // "0x...."
        public Dictionary<string, string> UIntPatchData { get; set; } // "0xOFFSET" -> "0xVALUE"
        public Dictionary<string, string> BytePatchData { get; set; } // "0xOFFSET" -> "0xVALUE"
    }
}
