﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace com.clusterrr.Famicom.Containers
{
    /// <summary>
    /// UNIF file container for NES/Famicom games
    /// </summary>
    public class UnifFile : IEnumerable<KeyValuePair<string, IEnumerable<byte>>>
    {
        /// <summary>
        /// UNIF fields
        /// </summary>
        private Dictionary<string, byte[]> fields = new Dictionary<string, byte[]>();

        /// <summary>
        /// UNIF version
        /// </summary>
        public uint Version { get; set; } = 5;

        /// <summary>
        /// Get/set UNIF field
        /// </summary>
        /// <param name="key">UNIF data block key</param>
        /// <returns></returns>
        public IEnumerable<byte> this[string key]
        {
            get
            {
                if (key.Length != 4) throw new ArgumentException("UNIF data block key must be 4 characters long");
                if (!ContainsField(key)) throw new IndexOutOfRangeException($"There is no {key} field");
                return Array.AsReadOnly(fields[key]);
            }
            set
            {
                if (key.Length != 4) throw new ArgumentException("UNIF data block key must be 4 characters long");
                if (value == null)
                    this.RemoveField(key);
                else
                    fields[key] = (value ?? new byte[0]).ToArray();
            }
        }

        /// <summary>
        /// Returns true if field exists in the UNIF
        /// </summary>
        /// <param name="fieldName">Field code</param>
        /// <returns>True if field exists in the UNIF</returns>
        public bool ContainsField(string fieldName) => fields.ContainsKey(fieldName);

        /// <summary>
        /// Remove field from the UNIF
        /// </summary>
        /// <param name="fieldName"></param>
        public void RemoveField(string fieldName) => fields.Remove(fieldName);

        /// <summary>
        /// Returns enumerator that iterates throught fields
        /// </summary>
        /// <returns>IEnumerable object</returns>
        public IEnumerator<KeyValuePair<string, IEnumerable<byte>>> GetEnumerator()
            => fields.Select(kv => new KeyValuePair<string, IEnumerable<byte>>(kv.Key, Array.AsReadOnly(kv.Value))).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Constructor to create empty UnifFile object
        /// </summary>
        public UnifFile()
        {
            DumpDate = DateTime.Now;
        }

        /// <summary>
        /// Create UnifFile object from raw .unf file contents
        /// </summary>
        /// <param name="data">Raw UNIF data</param>
        public UnifFile(byte[] data)
        {
            var header = new byte[32];
            Array.Copy(data, header, 32);
            if (header[0] != 'U' || header[1] != 'N' || header[2] != 'I' || header[3] != 'F')
                throw new InvalidDataException("Invalid UNIF header");
            Version = BitConverter.ToUInt32(header, 4);
            int pos = 32;
            while (pos < data.Length)
            {
                var type = Encoding.UTF8.GetString(data, pos, 4);
                pos += 4;
                int length = BitConverter.ToInt32(data, pos);
                pos += 4;
                var fieldData = new byte[length];
                Array.Copy(data, pos, fieldData, 0, length);
                this[type] = fieldData;
                pos += length;
            }
        }

        /// <summary>
        /// Create UnifFile object from specified file
        /// </summary>
        /// <param name="fileName">Path to the .unf file</param>
        public UnifFile(string fileName) : this(File.ReadAllBytes(fileName))
        {
        }

        /// <summary>
        /// Create UnifFile object from raw .unf file contents
        /// </summary>
        /// <param name="data"></param>
        /// <returns>UnifFile object</returns>
        public static UnifFile FromBytes(byte[] data) => new UnifFile(data);

        /// <summary>
        /// Create UnifFile object from specified file
        /// </summary>
        /// <param name="filename">Path to the .unf file</param>
        /// <returns>UnifFile object</returns>
        public static UnifFile FromFile(string filename) => new UnifFile(filename);

        /// <summary>
        /// Returns .unf file contents
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            // Some checks
            if (ContainsField("CTRL") && Version < 7)
                throw new InvalidDataException("CTRL (controllers) field requires UNIF version 7 or greater");
            if (ContainsField("TVCI") && Version < 6)
                throw new InvalidDataException("TVCI (controllers) field requires UNIF version 6 or greater");

            var data = new List<byte>();
            // Header
            data.AddRange(Encoding.UTF8.GetBytes("UNIF"));
            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(Enumerable.Repeat<byte>(0, 24).ToArray());
            // Fields
            foreach (var kv in this)
            {
                data.AddRange(Encoding.UTF8.GetBytes(kv.Key));
                var v = kv.Value.ToArray();
                int len = v.Length;
                data.AddRange(BitConverter.GetBytes(len));
                data.AddRange(v);
            }
            return data.ToArray();
        }

        /// <summary>
        /// Save as .unf file
        /// </summary>
        /// <param name="filename">Target filename</param>
        public void Save(string filename) => File.WriteAllBytes(filename, ToBytes());

        /// <summary>
        /// Convert string to null-terminated UTF string
        /// </summary>
        /// <param name="text">Input text</param>
        /// <returns>Output byte[] array</returns>
        private static byte[] StringToUTF8N(string text)
        {
            var str = Encoding.UTF8.GetBytes(text);
            var result = new byte[str.Length + 1];
            Array.Copy(str, result, str.Length);
            return result;
        }

        /// <summary>
        /// Convert null-terminated UTF string to string
        /// </summary>
        /// <param name="data">Input array of bytes</param>
        /// <param name="maxLength">Maximum number of bytes to parse</param>
        /// <param name="offset">Start offset</param>
        /// <returns></returns>
        private static string UTF8NToString(byte[] data, int maxLength = int.MaxValue, int offset = 0)
        {
            int length = 0;
            while ((data[length + offset] != 0) && (length + offset < data.Length) && (length < maxLength))
                length++;
            return Encoding.UTF8.GetString(data, offset, length);
        }

        /// <summary>
        /// Mapper name
        /// </summary>
        public string? Mapper
        {
            get => ContainsField("MAPR") ? UTF8NToString(this["MAPR"].ToArray()) : null;
            set
            {
                if (value == null)
                    RemoveField("MAPR");
                else
                    this["MAPR"] = StringToUTF8N(value);
            }
        }

        /// <summary>
        /// The dumper name
        /// </summary>
        /// 
        public string? DumperName
        {
            get
            {
                if (!ContainsField("DINF"))
                    return null;
                var data = this["DINF"].ToArray();
                if (data.Length >= 204 && data[0] != 0)
                    return UTF8NToString(data, 100);
                else
                    return null;
            }
            set
            {
                if (value != null || DumpingSoftware != null || DumpDate != null)
                {
                    if (!ContainsField("DINF"))
                        this["DINF"] = new byte[204];
                    var data = this["DINF"].ToArray();
                    for (int i = 0; i < 100; i++)
                        data[i] = 0;
                    if (value != null)
                    {
                        var name = StringToUTF8N(value);
                        Array.Copy(name, 0, data, 0, Math.Min(100, name!.Length));
                    }
                    this["DINF"] = data;
                }
                else RemoveField("DINF");
            }
        }

        /// <summary>
        /// The name of the dumping software or mechanism
        /// </summary>
        public string? DumpingSoftware
        {
            get
            {
                if (!ContainsField("DINF"))
                    return null;
                var data = this["DINF"].ToArray();
                if (data.Length >= 204 && data[0] != 0)
                    return UTF8NToString(data, 100, 104);
                else
                    return null;
            }
            set
            {
                if (value != null || DumperName != null || DumpDate != null)
                {
                    if (!ContainsField("DINF"))
                        this["DINF"] = new byte[204];
                    var data = this["DINF"].ToArray();
                    for (int i = 104; i < 104 + 100; i++)
                        data[i] = 0;
                    if (value != null)
                    {
                        var name = StringToUTF8N(value);
                        Array.Copy(name, 0, data, 104, Math.Min(100, name!.Length));
                    }
                    this["DINF"] = data;
                }
                else RemoveField("DINF");
            }
        }

        /// <summary>
        /// Date of the dump
        /// </summary>
        public DateTime? DumpDate
        {
            get
            {
                if (!ContainsField("DINF")) return null;
                var data = this["DINF"].ToArray();
                if (data[0] == 0 && data[1] == 0 && data[2] == 0 && data[3] == 0)
                    return null;
                return new DateTime(
                        year: data[102] | (data[103] << 8),
                        month: data[101],
                        day: data[100]
                    );
            }
            set
            {
                if (value != null || DumperName != null || DumpingSoftware != null)
                {
                    if (!ContainsField("DINF"))
                        this["DINF"] = new byte[204];
                    var data = this["DINF"].ToArray();
                    if (value != null)
                    {
                        data[100] = (byte)value.Value.Day;
                        data[101] = (byte)value.Value.Month;
                        data[102] = (byte)(value.Value.Year & 0xFF);
                        data[103] = (byte)(value.Value.Year >> 8);
                    }
                    else
                    {
                        // Is it valid?
                        data[100] = 0;
                        data[101] = 0;
                        data[102] = 0;
                        data[103] = 0;
                    }
                    this["DINF"] = data;
                }
                else RemoveField("DINF");
            }
        }

        /// <summary>
        /// Name of the game
        /// </summary>
        public string? GameName
        {
            get => ContainsField("NAME") ? UTF8NToString(this["NAME"].ToArray()) : null;
            set
            {
                if (value == null)
                    RemoveField("NAME");
                else
                    this["NAME"] = StringToUTF8N(value!);
            }
        }

        /// <summary>
        /// For non-homebrew NES/Famicom games, this field's value is always a function of the region in which a game was released
        /// </summary>
        public Timing? Region
        {
            get
            {
                if (ContainsField("TVCI") && this["TVCI"].Any())
                    return (Timing)this["TVCI"].First();
                else
                    return null;
            }
            set
            {
                if (value != null)
                    this["TVCI"] = new byte[] { (byte)value };
                else
                    RemoveField("TVCI");
            }
        }

        /// <summary>
        /// Controllers usable by this game (bitmask)
        /// </summary>
        public Controller? Controllers
        {
            get
            {
                if (ContainsField("CTRL") && this["CTRL"].Any())
                    return (Controller)this["CTRL"].First();
                else
                    return Controller.None;
            }
            set
            {
                if (value != null)
                    fields["CTRL"] = new byte[] { (byte)value };
                else
                    RemoveField("CTRL");
            }
        }

        /// <summary>
        /// Battery-backed (or other non-volatile memory) memory is present
        /// </summary>
        public bool? Battery
        {
            get
            {
                if (ContainsField("BATR") && this["BATR"].Any())
                    return this["BATR"].First() != 0;
                else
                    return false;
            }
            set
            {
                if (value != null)
                    fields["BATR"] = new byte[] { (byte)((bool)value ? 1 : 0) };
                else
                    RemoveField("BATR");
            }
        }

        /// <summary>
        /// Mirroring type
        /// </summary>
        public MirroringType? Mirroring
        {
            get
            {
                if (ContainsField("MIRR") && this["MIRR"].Any())
                    return (MirroringType)this["MIRR"].First();
                else
                    return null;
            }
            set
            {
                if (value != null && value != MirroringType.Unknown)
                    this["MIRR"] = new byte[] { (byte)value };
                else
                    this.RemoveField("MIRR");
            }
        }

        /// <summary>
        /// Calculate CRC32 for PRG and CHR fields and store it into PCKx and CCKx fields
        /// </summary>
        public void CalculateAndStoreCRCs()
        {
            foreach (var kv in this.Where(kv => kv.Key.StartsWith("PRG")))
            {
                var num = kv.Key[3];
                var crc32 = Crc32Calculator.CalculateCRC32(kv.Value.ToArray());
                this[$"PCK{num}"] = BitConverter.GetBytes(crc32);
            }
            foreach (var kv in this.Where(kv => kv.Key.StartsWith("CHR")))
            {
                var num = kv.Key[3];
                var crc32 = Crc32Calculator.CalculateCRC32(kv.Value.ToArray());
                this[$"CCK{num}"] = BitConverter.GetBytes(crc32);
            }
        }

        /// <summary>
        /// Calculate overall CRC32
        /// </summary>
        /// <returns></returns>
        public uint CalculateCRC32()
            => Crc32Calculator.CalculateCRC32(
                Enumerable.Concat(fields.Where(k => k.Key.StartsWith("PRG")).OrderBy(k => k.Key).SelectMany(i => i.Value),
                                  fields.Where(k => k.Key.StartsWith("CHR")).OrderBy(k => k.Key).SelectMany(i => i.Value)).ToArray()
            );

        /// <summary>
        /// Default game controller(s)
        /// </summary>
        [Flags]
        public enum Controller
        {
            /// <summary>
            /// None
            /// </summary>
            None = 0,
            /// <summary>
            /// Standatd Controller
            /// </summary>
            StandardController = 1,
            /// <summary>
            /// Zapper
            /// </summary>
            Zapper = 2,
            /// <summary>
            /// R.O.B.
            /// </summary>
            ROB = 4,
            /// <summary>
            /// Arkanoid Controller
            /// </summary>
            ArkanoidController = 8,
            /// <summary>
            /// Power Pad
            /// </summary>
            PowerPad = 16,
            /// <summary>
            /// Four Score
            /// </summary>
            FourScore = 32,
        }
    }
}
