using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace com.clusterrr.Famicom.Containers
{
    /// <summary>
    /// UNIF file container for NES/Famicom games
    /// </summary>
    public class UnifFile : IEnumerable<KeyValuePair<string, byte[]>>
    {
        private const string FIELD_CTRL = "CTRL";
        private const string FIELD_TVCI = "TVCI";
        private const string FIELD_DINF = "DINF";
        private const string FIELD_NAME = "NAME";
        private const string FIELD_BATR = "BATR";
        private const string FIELD_MAPR = "MAPR";
        private const string FIELD_MIRR = "MIRR";
        private const string FIELD_PRG0 = "PRG0";
        private const string FIELD_PRG1 = "PRG1";
        private const string FIELD_PRG2 = "PRG2";
        private const string FIELD_PRG3 = "PRG3";
        private const string FIELD_CHR0 = "CHR0";
        private const string FIELD_CHR1 = "CHR1";
        private const string FIELD_CHR2 = "CHR2";
        private const string FIELD_CHR3 = "CHR3";
        private const string PREFIX_PRG = "PRG";
        private const string PREFIX_CHR = "CHR";

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
        public byte[] this[string key]
        {
            get
            {
                if (key.Length != 4) throw new ArgumentException("UNIF data block key must be 4 characters long");
                if (!ContainsField(key)) throw new IndexOutOfRangeException($"There is no {key} field");
                return fields[key];
            }
            set
            {
                if (key.Length != 4) throw new ArgumentException("UNIF data block key must be 4 characters long");
                if (value == null)
                    this.RemoveField(key);
                else
                    fields[key] = value;
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
        public IEnumerator<KeyValuePair<string, byte[]>> GetEnumerator()
            => fields.Select(kv => new KeyValuePair<string, byte[]>(kv.Key, kv.Value)).GetEnumerator();

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
            if (ContainsField(FIELD_CTRL) && Version < 7)
                throw new InvalidDataException("CTRL (controllers) field requires UNIF version 7 or greater");
            if (ContainsField(FIELD_TVCI) && Version < 6)
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
                var value = kv.Value;
                int len = value.Length;
                data.AddRange(BitConverter.GetBytes(len));
                data.AddRange(value);
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
        /// Mapper name (null if none)
        /// </summary>
        public string? Mapper
        {
            get => ContainsField(FIELD_MAPR) ? UTF8NToString(this[FIELD_MAPR]) : null;
            set
            {
                if (value == null)
                    RemoveField(FIELD_MAPR);
                else
                    this[FIELD_MAPR] = StringToUTF8N(value);
            }
        }

        /// <summary>
        /// The dumper name (null if none)
        /// </summary>
        /// 
        public string? DumperName
        {
            get
            {
                if (!ContainsField(FIELD_DINF))
                    return null;
                var data = this[FIELD_DINF];
                if (data.Length >= 204 && data[0] != 0)
                    return UTF8NToString(data, 100);
                else
                    return null;
            }
            set
            {
                if (value != null || DumpingSoftware != null || DumpDate != null)
                {
                    if (!ContainsField(FIELD_DINF))
                        this[FIELD_DINF] = new byte[204];
                    var data = this[FIELD_DINF];
                    for (int i = 0; i < 100; i++)
                        data[i] = 0;
                    if (value != null)
                    {
                        var name = StringToUTF8N(value);
                        Array.Copy(name, 0, data, 0, Math.Min(100, name!.Length));
                    }
                    this[FIELD_DINF] = data;
                }
                else RemoveField(FIELD_DINF);
            }
        }

        /// <summary>
        /// The name of the dumping software or mechanism (null if none)
        /// </summary>
        public string? DumpingSoftware
        {
            get
            {
                if (!ContainsField(FIELD_DINF))
                    return null;
                var data = this[FIELD_DINF];
                if (data.Length >= 204 && data[0] != 0)
                    return UTF8NToString(data, 100, 104);
                else
                    return null;
            }
            set
            {
                if (value != null || DumperName != null || DumpDate != null)
                {
                    if (!ContainsField(FIELD_DINF))
                        this[FIELD_DINF] = new byte[204];
                    var data = this[FIELD_DINF];
                    for (int i = 104; i < 104 + 100; i++)
                        data[i] = 0;
                    if (value != null)
                    {
                        var name = StringToUTF8N(value);
                        Array.Copy(name, 0, data, 104, Math.Min(100, name!.Length));
                    }
                    this[FIELD_DINF] = data;
                }
                else RemoveField(FIELD_DINF);
            }
        }

        /// <summary>
        /// Date of the dump (null if none)
        /// </summary>
        public DateTime? DumpDate
        {
            get
            {
                if (!ContainsField(FIELD_DINF)) return null;
                var data = this[FIELD_DINF];
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
                    if (!ContainsField(FIELD_DINF))
                        this[FIELD_DINF] = new byte[204];
                    var data = this[FIELD_DINF];
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
                        data[100] = data[101] = data[102] = data[103] = 0;
                    }
                    this[FIELD_DINF] = data;
                }
                else RemoveField(FIELD_DINF);
            }
        }

        /// <summary>
        /// Name of the game (null if none)
        /// </summary>
        public string? GameName
        {
            get => ContainsField(FIELD_NAME) ? UTF8NToString(this[FIELD_NAME]) : null;
            set
            {
                if (value == null)
                    RemoveField(FIELD_NAME);
                else
                    this[FIELD_NAME] = StringToUTF8N(value!);
            }
        }

        /// <summary>
        /// For non-homebrew NES/Famicom games, this field's value is always a function of the region in which a game was released (null if none)
        /// </summary>
        public Timing? Region
        {
            get
            {
                if (ContainsField(FIELD_TVCI) && this[FIELD_TVCI].Any())
                    return (Timing)this[FIELD_TVCI].First();
                else
                    return null;
            }
            set
            {
                if (value != null)
                    this[FIELD_TVCI] = new byte[] { (byte)value };
                else
                    RemoveField(FIELD_TVCI);
            }
        }

        /// <summary>
        /// Controllers usable by this game, bitmask (null if none)
        /// </summary>
        public Controller? Controllers
        {
            get
            {
                if (ContainsField(FIELD_CTRL) && this[FIELD_CTRL].Any())
                    return (Controller)this[FIELD_CTRL].First();
                else
                    return Controller.None;
            }
            set
            {
                if (value != null)
                    fields[FIELD_CTRL] = new byte[] { (byte)value };
                else
                    RemoveField(FIELD_CTRL);
            }
        }

        /// <summary>
        /// Battery-backed (or other non-volatile memory) memory is present (null if none)
        /// </summary>
        public bool? Battery
        {
            get
            {
                if (ContainsField(FIELD_BATR) && this[FIELD_BATR].Any())
                    return this[FIELD_BATR].First() != 0;
                else
                    return false;
            }
            set
            {
                if (value != null)
                    fields[FIELD_BATR] = new byte[] { (byte)((bool)value ? 1 : 0) };
                else
                    RemoveField(FIELD_BATR);
            }
        }

        /// <summary>
        /// Mirroring type (null if none)
        /// </summary>
        public MirroringType? Mirroring
        {
            get
            {
                if (ContainsField(FIELD_MIRR) && this[FIELD_MIRR].Any())
                    return (MirroringType)this[FIELD_MIRR].First();
                else
                    return null;
            }
            set
            {
                if (value != null && value != MirroringType.Unknown)
                    this[FIELD_MIRR] = new byte[] { (byte)value };
                else
                    this.RemoveField(FIELD_MIRR);
            }
        }

        /// <summary>
        /// PRG0 field (null if none)
        /// </summary>
        public byte[]? PRG0
        {
            get
            {
                if (ContainsField(FIELD_PRG0))
                    return this[FIELD_PRG0];
                else
                    return null;
            }
            set
            {
                if (value != null)
                    this[FIELD_PRG0] = value;
                else
                    RemoveField(FIELD_PRG0);
            }
        }

        /// <summary>
        /// PRG1 field (null if none)
        /// </summary>
        public byte[]? PRG1
        {
            get
            {
                if (ContainsField(FIELD_PRG1))
                    return this[FIELD_PRG1];
                else
                    return null;
            }
            set
            {
                if (value != null)
                    this[FIELD_PRG1] = value;
                else
                    RemoveField(FIELD_PRG1);
            }
        }

        /// <summary>
        /// PRG2 field (null if none)
        /// </summary>
        public byte[]? PRG2
        {
            get
            {
                if (ContainsField(FIELD_PRG2))
                    return this[FIELD_PRG2];
                else
                    return null;
            }
            set
            {
                if (value != null)
                    this[FIELD_PRG2] = value;
                else
                    RemoveField(FIELD_PRG2);
            }
        }

        /// <summary>
        /// PRG3 field (null if none)
        /// </summary>
        public byte[]? PRG3
        {
            get
            {
                if (ContainsField(FIELD_PRG3))
                    return this[FIELD_PRG3];
                else
                    return null;
            }
            set
            {
                if (value != null)
                    this[FIELD_PRG3] = value;
                else
                    RemoveField(FIELD_PRG3);
            }
        }

        /// <summary>
        /// CHR0 field (null if none)
        /// </summary>
        public byte[]? CHR0
        {
            get
            {
                if (ContainsField(FIELD_CHR0))
                    return this[FIELD_CHR0];
                else
                    return null;
            }
            set
            {
                if (value != null)
                    this[FIELD_CHR0] = value;
                else
                    RemoveField(FIELD_CHR0);
            }
        }

        /// <summary>
        /// CHR1 field (null if none)
        /// </summary>
        public byte[]? CHR1
        {
            get
            {
                if (ContainsField(FIELD_CHR1))
                    return this[FIELD_CHR1];
                else
                    return null;
            }
            set
            {
                if (value != null)
                    this[FIELD_CHR1] = value;
                else
                    RemoveField(FIELD_CHR1);
            }
        }

        /// <summary>
        /// CHR2 field (null if none)
        /// </summary>
        public byte[]? CHR2
        {
            get
            {
                if (ContainsField(FIELD_CHR2))
                    return this[FIELD_CHR2];
                else
                    return null;
            }
            set
            {
                if (value != null)
                    this[FIELD_CHR2] = value;
                else
                    RemoveField(FIELD_CHR2);
            }
        }

        /// <summary>
        /// CHR3 field (null if none)
        /// </summary>
        public byte[]? CHR3
        {
            get
            {
                if (ContainsField(FIELD_CHR3))
                    return this[FIELD_CHR3];
                else
                    return null;
            }
            set
            {
                if (value != null)
                    this[FIELD_CHR3] = value;
                else
                    RemoveField(FIELD_CHR3);
            }
        }

        /// <summary>
        /// Calculate CRC32 for PRG and CHR fields and store it into PCKx and CCKx fields
        /// </summary>
        public void CalculateAndStoreCRCs()
        {
            foreach (var kv in this.Where(kv => kv.Key.StartsWith(PREFIX_PRG)))
            {
                var num = kv.Key[3];
                using var crc32 = new Crc32();
                var crc32sum = crc32.ComputeHash(kv.Value);
                this[$"PCK{num}"] = crc32sum;
            }
            foreach (var kv in this.Where(kv => kv.Key.StartsWith(PREFIX_CHR)))
            {
                var num = kv.Key[3];
                using var crc32 = new Crc32();
                var crc32sum = crc32.ComputeHash(kv.Value);
                this[$"CCK{num}"] = crc32sum;
            }
        }

        /// <summary>
        /// Calculate MD5 checksum of ROM (all PRG fields + all CHR fields)
        /// </summary>
        /// <returns>MD5 checksum for all PRG and CHR data</returns>
        public byte[] CalculateMD5()
        {
            using var md5 = MD5.Create();
            foreach(var kv in fields.Where(k => k.Key.StartsWith(PREFIX_PRG)).OrderBy(k => k.Key)
                .Concat(fields.Where(k => k.Key.StartsWith(PREFIX_CHR)).OrderBy(k => k.Key)))
            {
                var v = kv.Value;
                int sizeUpPow2 = 0;
                if (v.Length > 0)
                {
                    sizeUpPow2 = 1;
                    while (sizeUpPow2 < v.Length) sizeUpPow2 *= 2;
                }
                md5.TransformBlock(v, 0, v.Length, null, 0);
                md5.TransformBlock(Enumerable.Repeat<byte>(byte.MaxValue, sizeUpPow2 - v.Length).ToArray(), 0, sizeUpPow2 - v.Length, null, 0);
            }
            md5.TransformFinalBlock(new byte[0], 0, 0);
            return md5.Hash;
        }

        /// <summary>
        /// Calculate CRC32 checksum of ROM (all PRG fields + all CHR fields)
        /// </summary>
        /// <returns>CRC32 checksum for all PRG and CHR data</returns>
        public uint CalculateCRC32()
        {
            using var crc32 = new Crc32();
            foreach (var kv in fields.Where(k => k.Key.StartsWith(PREFIX_PRG)).OrderBy(k => k.Key)
                .Concat(fields.Where(k => k.Key.StartsWith(PREFIX_CHR)).OrderBy(k => k.Key)))
            {
                var v = kv.Value;
                int sizeUpPow2 = 0;
                if (v.Length > 0)
                {
                    sizeUpPow2 = 1;
                    while (sizeUpPow2 < v.Length) sizeUpPow2 *= 2;
                }
                crc32.TransformBlock(v, 0, v.Length, null, 0);
                crc32.TransformBlock(Enumerable.Repeat<byte>(byte.MaxValue, sizeUpPow2 - v.Length).ToArray(), 0, sizeUpPow2 - v.Length, null, 0);
            }
            crc32.TransformFinalBlock(new byte[0], 0, 0);
            return BitConverter.ToUInt32(crc32.Hash.Reverse().ToArray(), 0);
        }

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
