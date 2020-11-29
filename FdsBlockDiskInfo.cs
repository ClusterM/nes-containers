using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.Famicom.Containers
{
    [StructLayout(LayoutKind.Sequential, Size = 58, Pack = 1, CharSet = CharSet.Ansi)]
    public class FdsBlockDiskInfo : IFdsBlock
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
            Japan = 0x49
        }

        [MarshalAs(UnmanagedType.U1)]
        // Raw byte: 0x01
        private byte blockType = 1;
        /// <summary>
        /// True if block type ID is valid
        /// </summary>
        public bool IsValid { get => blockType == 1; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        char[] diskVerification = "*NINTENDO-HVC*".ToCharArray();
        /// <summary>
        /// Literal ASCII string: *NINTENDO-HVC*
        /// </summary>
        public string DiskVerification { get => new string(diskVerification).Trim(new char[] { '\0', ' ' }); /*set => diskVerification = value.PadRight(14).ToCharArray(0, value.Length > 14 ? 14 : value.Length);*/ }

        [MarshalAs(UnmanagedType.U1)]
        private byte manufacturerCode;
        /// <summary>
        /// Manufacturer code. $00 = Unlicensed, $01 = Nintendo
        /// </summary>
        public byte ManufacturerCode { get => manufacturerCode; set => manufacturerCode = value; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        char[] gameName;
        /// <summary>
        /// 3-letter ASCII code per game (e.g. ZEL for The Legend of Zelda)
        /// </summary>
        public string GameName { get => new string(gameName).Trim(new char[] { '\0', ' ' }); set => gameName = value.PadRight(3).ToCharArray(0, value.Length > 3 ? 3 : value.Length); }

        [MarshalAs(UnmanagedType.U1)]
        char gameType;
        /// <summary>
        /// $20 = " " — Normal disk
        /// $45 = "E" — Event(e.g.Japanese national DiskFax tournaments)
        /// $52 = "R" — Reduction in price via advertising
        /// </summary>
        public char GameType { get => gameType; set => gameType = value; }

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
        /// Disk type. $00 = FMC ("normal card"), $01 = FSC ("card with shutter"). May correlate with FMC and FSC product codes
        /// </summary>
        public DiskTypes DiskType { get => (DiskTypes)diskType; set => diskType = (byte)value; }

        [MarshalAs(UnmanagedType.U1)]
        // Speculative: (Err.10) Possibly indicates disk #; usually $00
        // Speculative: $00 = yellow disk, $01 = blue or gold disk, $FE = white disk, $FF = blue disk
        byte unknown01 = 0x00;
        [MarshalAs(UnmanagedType.U1)]
        byte bootFile;
        /// <summary>
        /// Boot read file code. Refers to the file code/file number to load upon boot/start-up
        /// </summary>
        public byte BootFile { get => bootFile; set => bootFile = value; }
        [MarshalAs(UnmanagedType.U1)]
        byte unknown02 = 0xFF;
        [MarshalAs(UnmanagedType.U1)]
        byte unknown03 = 0xFF;
        [MarshalAs(UnmanagedType.U1)]
        byte unknown04 = 0xFF;
        [MarshalAs(UnmanagedType.U1)]
        byte unknown05 = 0xFF;
        [MarshalAs(UnmanagedType.U1)]
        byte unknown06 = 0xFF;

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
        // $49 = Japan
        byte countryCode = (byte)Country.Japan;
        /// <summary>
        /// Country code. $49 = Japan
        /// </summary>
        public Country CountryCode { get => (Country)countryCode; set => countryCode = (byte)value; }

        [MarshalAs(UnmanagedType.U1)]
        // Raw byte: $61. Speculative: Region code?
        byte unknown07 = 0x61;
        [MarshalAs(UnmanagedType.U1)]
        // Raw byte: $00. Speculative: Location/site?
        byte unknown08 = 0x00;
        [MarshalAs(UnmanagedType.U1)]
        // Raw byte: $00
        byte unknown09 = 0x00;
        [MarshalAs(UnmanagedType.U1)]
        // Raw byte: $02
        // Speculative: some kind of game information representation?
        byte unknown10 = 0x02;

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
        byte unknown11 = 0x00;
        [MarshalAs(UnmanagedType.U1)]
        // Raw byte: $80
        byte unknown12 = 0x80;
        [MarshalAs(UnmanagedType.U2)]

        ushort diskWriterSerialNumber;
        /// <summary>
        /// Disk Writer serial number
        /// </summary>
        public ushort DiskWriterSerialNumber { get => diskWriterSerialNumber; set => diskWriterSerialNumber = value; }

        [MarshalAs(UnmanagedType.U1)]
        // Raw byte: $07
        byte unknown13 = 0x07;

        [MarshalAs(UnmanagedType.U1)]
        byte diskRewriteCount = 0x00;
        /// <summary>
        /// Disk rewrite count. $00 = Original (no copies)
        /// </summary>
        public byte DiskRewriteCount
        {
            get
            {
                return (byte)((diskRewriteCount & 0x0F) + ((diskRewriteCount >> 4) & 0x0F) * 10);
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
        byte unknown14 = 0x00;

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
    }
}
