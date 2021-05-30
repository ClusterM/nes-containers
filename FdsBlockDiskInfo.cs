using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace com.clusterrr.Famicom.Containers
{
    [StructLayout(LayoutKind.Sequential, Size = 58, Pack = 1)]
    public class FdsBlockDiskInfo : IFdsBlock, IEquatable<FdsBlockDiskInfo>
    {
        public enum DiskSides
        {
            A = 0,
            B = 1,
        }
        public enum DiskTypes
        {
            FMS = 0, // Normal
            FSC = 1, // With shutter
        }
        public enum Country
        {
            Japan = 0x49,
        }

        public enum Manufacturer
        {
            Unlicensed = 0x00,
            Nintendo = 0x01,
            Capcom = 0x08,
            Jaleco = 0x0A,
            Hudson_Soft = 0x18,
            Irem = 0x49,
            Gakken = 0x4A,
            BulletProof_Software = 0x8B,
            PackInVideo = 0x99,
            Tecmo = 0x9B,
            Imagineer = 0x9C,
            Scorpion_Soft = 0xA2,
            Konami = 0xA4,
            Kawada_Co = 0xA6,
            Takara = 0xA7,
            Royal_Industries = 0xA8,
            Toei_Animation = 0xAC,
            Namco = 0xAF,
            ASCII_Corporation = 0xB1,
            Bandai = 0xB2,
            Soft_Pro_Inc = 0xB3,
            HAL_Laboratory = 0xB6,
            Sunsoft_and_Ask_Co = 0xBB,
            Toshiba_EMI = 0xBC,
            Taito = 0xC0,
            Sunsoft = 0xC1,
            Kemco = 0xC2,
            Square = 0xC3,
            Tokuma_Shoten = 0xC4,
            Data_East = 0xC5,
            Tonkin_House_and_Tokyo_Shoseki = 0xC6,
            East_Cube = 0xC7,
            Konami_and_Ultra_and_Palcom = 0xCA,
            NTVIC_and_VAP = 0xCB,
            Use_Co = 0xCC,
            Pony_Canyon_and_FCI = 0xCE,
            Sofel = 0xD1,
            Bothtec_Inc = 0xD2,
            Hiro_Co = 0xDB,
            Athena = 0xE7,
            Atlus = 0xEB,
        }

        [MarshalAs(UnmanagedType.U1)]
        // Raw byte: 0x01
        private readonly byte blockType = 1;
        public byte ValidTypeID { get => 1; }
        /// <summary>
        /// True if block type ID is valid
        /// </summary>
        public bool IsValid { get => blockType == ValidTypeID; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        readonly byte[] diskVerification = Encoding.ASCII.GetBytes("*NINTENDO-HVC*");
        /// <summary>
        /// Literal ASCII string: *NINTENDO-HVC*
        /// </summary>
        public string DiskVerification => Encoding.ASCII.GetString(diskVerification).Trim(new char[] { '\0', ' ' }); /*set => diskVerification = value.PadRight(14).ToCharArray(0, value.Length > 14 ? 14 : value.Length);*/

        [MarshalAs(UnmanagedType.U1)]
        private byte manufacturerCode;
        /// <summary>
        /// Manufacturer code. = = 0x00, Unlicensed, = = 0x01, Nintendo
        /// </summary>
        public Manufacturer ManufacturerCode { get => (Manufacturer)manufacturerCode; set => manufacturerCode = (byte)value; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        byte[] gameName;
        /// <summary>
        /// 3-letter ASCII code per game (e.g. ZEL for The Legend of Zelda)
        /// </summary>
        public string GameName { get => Encoding.ASCII.GetString(gameName).Trim(new char[] { '\0', ' ' }); set => gameName = Encoding.ASCII.GetBytes(value.PadRight(3)).Take(3).ToArray(); }

        [MarshalAs(UnmanagedType.U1)]
        byte gameType;
        /// <summary>
        /// = = 0x20, " " — Normal disk
        /// = = 0x45, "E" — Event(e.g.Japanese national DiskFax tournaments)
        /// = = 0x52, "R" — Reduction in price via advertising
        /// </summary>
        public char GameType { get => (char)gameType; set => gameType = (byte)value; }

        [MarshalAs(UnmanagedType.U1)]
        byte gameVersion;
        /// <summary>
        /// Game version/revision number. Starts at $00, increments per revision
        /// </summary>
        public byte GameVersion { get => gameVersion; set => gameVersion = value; }

        [MarshalAs(UnmanagedType.U1)]
        byte diskSide;
        /// <summary>
        /// Side number. Single-sided disks use A
        /// </summary>
        public DiskSides DiskSide { get => (DiskSides)diskSide; set => diskSide = (byte)value; }

        [MarshalAs(UnmanagedType.U1)]
        byte diskNumber;
        /// <summary>
        /// Disk number. First disk is $00, second is $01, etc.
        /// </summary>
        public byte DiskNumber { get => diskNumber; set => diskNumber = value; }

        [MarshalAs(UnmanagedType.U1)]
        byte diskType;
        /// <summary>
        /// Disk type. = = 0x00, FMC ("normal card"), = = 0x01, FSC ("card with shutter"). May correlate with FMC and FSC product codes
        /// </summary>
        public DiskTypes DiskType { get => (DiskTypes)diskType; set => diskType = (byte)value; }

        [MarshalAs(UnmanagedType.U1)]
        // Speculative: (Err.10) Possibly indicates disk #; usually $00
        // Speculative: = = 0x00, yellow disk, = = 0x01, blue or gold disk, = = 0xFE, white disk, = = 0xFF, blue disk
        readonly byte unknown01 = 0x00;
        [MarshalAs(UnmanagedType.U1)]
        byte bootFile;
        /// <summary>
        /// Boot read file code. Refers to the file code/file number to load upon boot/start-up
        /// </summary>
        public byte BootFile { get => bootFile; set => bootFile = value; }
        [MarshalAs(UnmanagedType.U1)]
        readonly byte unknown02 = 0xFF;
        [MarshalAs(UnmanagedType.U1)]
        readonly byte unknown03 = 0xFF;
        [MarshalAs(UnmanagedType.U1)]
        readonly byte unknown04 = 0xFF;
        [MarshalAs(UnmanagedType.U1)]
        readonly byte unknown05 = 0xFF;
        [MarshalAs(UnmanagedType.U1)]
        readonly byte unknown06 = 0xFF;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        byte[] manufacturingDate;
        /// <summary>
        /// Manufacturing date
        /// </summary>
        public DateTime ManufacturingDate
        {
            get
            {
                try
                {
                    return new DateTime(
                        ((manufacturingDate[0] & 0x0F) + ((manufacturingDate[0] >> 4) & 0x0F) * 10) + 1925,
                        ((manufacturingDate[1] & 0x0F) + ((manufacturingDate[1] >> 4) & 0x0F) * 10),
                        ((manufacturingDate[2] & 0x0F) + ((manufacturingDate[2] >> 4) & 0x0F) * 10)
                        );
                }
                catch
                {
                    return new DateTime();
                }
            }
            set
            {
                manufacturingDate = new byte[]
                {
                    (byte)(((value.Year - 1925) % 10) | (((value.Year - 1925) / 10) << 4)),
                    (byte)(((value.Month) % 10) | (((value.Month) / 10) << 4)),
                    (byte)(((value.Day) % 10) | (((value.Day) / 10) << 4))
                };
            }
        }

        [MarshalAs(UnmanagedType.U1)]
        // = = 0x49, Japan
        byte countryCode = (byte)Country.Japan;
        /// <summary>
        /// Country code. = = 0x49, Japan
        /// </summary>
        public Country CountryCode { get => (Country)countryCode; set => countryCode = (byte)value; }

        [MarshalAs(UnmanagedType.U1)]
        // Raw byte: $61. Speculative: Region code?
        readonly byte unknown07 = 0x61;
        [MarshalAs(UnmanagedType.U1)]
        // Raw byte: $00. Speculative: Location/site?
        readonly byte unknown08 = 0x00;
        [MarshalAs(UnmanagedType.U2)]
        // Raw bytes: $00 $02
        readonly ushort unknown09 = 0x0200;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        // Speculative: some kind of game information representation?
        readonly byte[] unknown10;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        byte[] rewrittenDate;
        /// <summary>
        /// "Rewritten disk" date. It's speculated this refers to the date the disk was formatted and rewritten by something like a Disk Writer kiosk.
        /// In the case of an original (non-copied) disk, this should be the same as Manufacturing date
        /// </summary>
        public DateTime RewrittenDate
        {
            get
            {
                try
                {
                    return new DateTime(
                        ((rewrittenDate[0] & 0x0F) + ((rewrittenDate[0] >> 4) & 0x0F) * 10) + 1925,
                        ((rewrittenDate[1] & 0x0F) + ((rewrittenDate[1] >> 4) & 0x0F) * 10),
                        ((rewrittenDate[2] & 0x0F) + ((rewrittenDate[2] >> 4) & 0x0F) * 10)
                        );
                }
                catch
                {
                    return new DateTime();
                }
            }
            set
            {
                rewrittenDate = new byte[]
                {
                    (byte)(((value.Year - 1925) % 10) | (((value.Year - 1925) / 10) << 4)),
                    (byte)(((value.Month) % 10) | (((value.Month) / 10) << 4)),
                    (byte)(((value.Day) % 10) | (((value.Day) / 10) << 4))
                };
            }
        }

        [MarshalAs(UnmanagedType.U1)]
        readonly byte unknown11 = 0x00;
        [MarshalAs(UnmanagedType.U1)]
        // Raw byte: $80
        readonly byte unknown12 = 0x80;
        [MarshalAs(UnmanagedType.U2)]

        ushort diskWriterSerialNumber;
        /// <summary>
        /// Disk Writer serial number
        /// </summary>
        public ushort DiskWriterSerialNumber { get => diskWriterSerialNumber; set => diskWriterSerialNumber = value; }

        [MarshalAs(UnmanagedType.U1)]
        // Raw byte: $07
        readonly byte unknown13 = 0x07;

        [MarshalAs(UnmanagedType.U1)]
        byte diskRewriteCount = 0x00;
        /// <summary>
        /// Disk rewrite count. = = 0x00, Original (no copies)
        /// </summary>
        public byte DiskRewriteCount
        {
            get
            {
                return (diskRewriteCount == 0xFF) ? (byte)0 : (byte)((diskRewriteCount & 0x0F) + ((diskRewriteCount >> 4) & 0x0F) * 10);
            }
            set
            {
                diskRewriteCount = (byte)(((value) % 10) | (((value) / 10) << 4));
            }
        }

        [MarshalAs(UnmanagedType.U1)]
        byte actualDiskSide = 0x00;
        /// <summary>
        /// Actual disk side
        /// </summary>
        public DiskSides ActualDiskSide { get => (DiskSides)actualDiskSide; set => actualDiskSide = (byte)value; }

        [MarshalAs(UnmanagedType.U1)]
        readonly byte unknown14 = 0x00;

        [MarshalAs(UnmanagedType.U1)]
        byte price = 0x00;
        /// <summary>
        /// Price code
        /// </summary>
        public byte Price { get => price; set => price = value; }

        [MarshalAs(UnmanagedType.U1)]
        private bool crcOk = true;
        /// <summary>
        /// Set by dumper. True when checksum is ok
        /// </summary>
        public bool CrcOk { get => crcOk; set => crcOk = value; }

        [MarshalAs(UnmanagedType.U1)]
        private bool endOfHeadMeet = false;
        /// <summary>
        /// Set by dumper. True when "end of head" flag was meet during dumping
        /// </summary>
        public bool EndOfHeadMeet { get => endOfHeadMeet; set => endOfHeadMeet = value; }

        public uint Length => 56;

        public static FdsBlockDiskInfo FromBytes(byte[] rawData, int position = 0)
        {
            int rawsize = Marshal.SizeOf(typeof(FdsBlockDiskInfo));
            if (rawsize > rawData.Length - position)
            {
                if (rawsize <= rawData.Length - position + 2)
                {
                    var newRawData = new byte[rawsize];
                    Array.Copy(rawData, position, newRawData, 0, rawsize - 2);
                    rawData = newRawData;
                    position = 0;
                }
                else
                {
                    throw new ArgumentException("Not enough data to fill FdsDiskInfoBlock class. Array length from position: " + (rawData.Length - position) + ", Struct length: " + rawsize);
                }
            }
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(rawData, position, buffer, rawsize);
            FdsBlockDiskInfo retobj = (FdsBlockDiskInfo)Marshal.PtrToStructure(buffer, typeof(FdsBlockDiskInfo));
            Marshal.FreeHGlobal(buffer);
            return retobj;
        }

        public byte[] ToBytes()
        {
            int rawSize = Marshal.SizeOf(this);
            IntPtr buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.StructureToPtr(this, buffer, false);
            byte[] rawDatas = new byte[rawSize - 2];
            Marshal.Copy(buffer, rawDatas, 0, rawSize - 2);
            Marshal.FreeHGlobal(buffer);
            return rawDatas;
        }

        public override string ToString() => GameName;

        public bool Equals(FdsBlockDiskInfo other)
        {
            return Enumerable.SequenceEqual(this.ToBytes(), other.ToBytes());
        }
    }
}
