using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace com.clusterrr.Famicom.Containers
{
    /// <summary>
    /// Disk info FDS block (block type 1)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 56, Pack = 1)]
    public class FdsBlockDiskInfo : IFdsBlock, IEquatable<FdsBlockDiskInfo>
    {
        /// <summary>
        /// Disk side
        /// </summary>
        public enum DiskSides
        {
            /// <summary>
            /// Side A
            /// </summary>
            A = 0,
            /// <summary>
            /// Side B
            /// </summary>
            B = 1,
        }
        /// <summary>
        /// Disk type
        /// </summary>
        public enum DiskTypes
        {
            /// <summary>
            /// Normal
            /// </summary>
            FMS = 0,
            /// <summary>
            /// With shutter
            /// </summary>
            FSC = 1,
        }
        /// <summary>
        /// Country
        /// </summary>
        public enum Country
        {
            /// <summary>
            /// Japan
            /// </summary>
            Japan = 0x49,
        }

        /// <summary>
        /// Company name, source: https://www.nesdev.org/wiki/Licensee_codes
        /// </summary>
        public enum Company
        {
            /// <summary>
            /// Nintendo
            /// </summary>
            Nintendo = 0x01,
            /// <summary>
            /// Nomura Securities? (unverified)
            /// </summary>
            NomuraSecurities = 0x07,
            /// <summary>
            /// Capcom
            /// </summary>
            Capcom = 0x08,
            /// <summary>
            /// Hot-B
            /// </summary>
            HotB = 0x09,
            /// <summary>
            /// Jaleco
            /// </summary>
            Jaleco = 0x0A,
            /// <summary>
            /// Coconuts Japan Entertainment
            /// </summary>
            CoconutsJapanEntertainment = 0x0B,
            /// <summary>
            /// Electronic Arts (Japan)
            /// </summary>
            ElectronicArtsJap = 0x13,
            /// <summary>
            /// Hudson Soft
            /// </summary>
            HudsonSoft = 0x18,
            /// <summary>
            /// Tokai Engineering
            /// </summary>
            TokaiEngineering = 0x21,
            /// <summary>
            /// Kemco (Japan)
            /// </summary>
            KemcoJap = 0x28,
            /// <summary>
            /// SETA (Japan)
            /// </summary>
            SetaJap = 0x29,
            /// <summary>
            /// Tamtex
            /// </summary>
            Tamtex = 0x2B,
            /// <summary>
            /// Hector Playing Interface (Hect)
            /// </summary>
            HectorPlayingInterface = 0x35,
            /// <summary>
            /// Loriciel
            /// </summary>
            Loriciel = 0x3D,
            /// <summary>
            /// Gremlin
            /// </summary>
            Gremlin = 0x3E,
            /// <summary>
            /// Seika Corporation
            /// </summary>
            SeikaCorporation = 0x40,
            /// <summary>
            /// Ubisoft
            /// </summary>
            Ubisoft = 0x41,
            /// <summary>
            /// System 3
            /// </summary>
            System3 = 0x46,
            /// <summary>
            /// Irem
            /// </summary>
            Irem = 0x49,
            /// <summary>
            /// Gakken
            /// </summary>
            Gakken = 0x4A,
            /// <summary>
            /// Absolute
            /// </summary>
            Absolute = 0x50,
            /// <summary>
            /// Acclaim (NA)
            /// </summary>
            AcclaimNA = 0x51,
            /// <summary>
            /// Activision
            /// </summary>
            Activision = 0x52,
            /// <summary>
            /// American Sammy
            /// </summary>
            AmericanSammy = 0x53,
            /// <summary>
            /// GameTek
            /// </summary>
            Gametek = 0x54,
            /// <summary>
            /// Hi Tech Expressions
            /// </summary>
            HITechExpressions = 0x55,
            /// <summary>
            /// LJN
            /// </summary>
            Ljn = 0x56,
            /// <summary>
            /// Matchbox Toys
            /// </summary>
            MatchboxToys = 0x57,
            /// <summary>
            /// Mattel
            /// </summary>
            Mattel = 0x58,
            /// <summary>
            /// Milton Bradley
            /// </summary>
            MiltonBradley = 0x59,
            /// <summary>
            /// Mindscape / Software Toolworks
            /// </summary>
            MindscapeSoftwareToolworks = 0x5A,
            /// <summary>
            /// SETA (NA)
            /// </summary>
            SetaNA = 0x5B,
            /// <summary>
            /// Taxan
            /// </summary>
            Taxan = 0x5C,
            /// <summary>
            /// Tradewest
            /// </summary>
            Tradewest = 0x5D,
            /// <summary>
            /// INTV Corporation
            /// </summary>
            IntvCorporation = 0x5E,
            /// <summary>
            /// Titus
            /// </summary>
            Titus = 0x60,
            /// <summary>
            /// Virgin Games
            /// </summary>
            VirginGames = 0x61,
            /// <summary>
            /// Ocean
            /// </summary>
            Ocean = 0x67,
            /// <summary>
            /// Electronic Arts (NA)
            /// </summary>
            ElectronicArtsNA = 0x69,
            /// <summary>
            /// Beam Software
            /// </summary>
            BeamSoftware = 0x6B,
            /// <summary>
            /// Elite Systems
            /// </summary>
            EliteSystems = 0x6E,
            /// <summary>
            /// Electro Brain
            /// </summary>
            ElectroBrain = 0x6F,
            /// <summary>
            /// Infogrames
            /// </summary>
            Infogrames = 0x70,
            /// <summary>
            /// JVC
            /// </summary>
            Jvc = 0x72,
            /// <summary>
            /// Parker Brothers
            /// </summary>
            ParkerBrothers = 0x73,
            /// <summary>
            /// The Sales Curve / SCi
            /// </summary>
            TheSalesCurveSci = 0x75,
            /// <summary>
            /// THQ
            /// </summary>
            Thq = 0x78,
            /// <summary>
            /// Accolade
            /// </summary>
            Accolade = 0x79,
            /// <summary>
            /// Triffix
            /// </summary>
            Triffix = 0x7A,
            /// <summary>
            /// Microprose Software
            /// </summary>
            MicroproseSoftware = 0x7C,
            /// <summary>
            /// Kemco (NA)
            /// </summary>
            KemcoNA = 0x7F,
            /// <summary>
            /// Misawa Entertainment
            /// </summary>
            MisawaEntertainment = 0x80,
            /// <summary>
            /// G. Amusements Co.
            /// </summary>
            GAmusementsCO = 0x83,
            /// <summary>
            /// G.O 1
            /// </summary>
            GO1 = 0x85,
            /// <summary>
            /// Tokuma Shoten Intermedia
            /// </summary>
            TokumaShotenIntermedia = 0x86,
            /// <summary>
            /// Nihon Maicom Kaihatsu (NMK)
            /// </summary>
            NihonMaicomKaihatsu = 0x89,
            /// <summary>
            /// BulletProof Software (BPS)
            /// </summary>
            BulletproofSoftware = 0x8B,
            /// <summary>
            /// VIC Tokai
            /// </summary>
            VicTokai = 0x8C,
            /// <summary>
            /// Sanritsu
            /// </summary>
            Sanritsu = 0x8D,
            /// <summary>
            /// Character Soft
            /// </summary>
            CharacterSoft = 0x8E,
            /// <summary>
            /// I'Max
            /// </summary>
            IMax = 0x8F,
            /// <summary>
            /// Toaplan
            /// </summary>
            Toaplan = 0x94,
            /// <summary>
            /// Varie
            /// </summary>
            Varie = 0x95,
            /// <summary>
            /// Yonezawa Party Room 21 / S'Pal
            /// </summary>
            YonezawaPartyRoom21SPal = 0x96,
            /// <summary>
            /// Pack-In-Video
            /// </summary>
            PackINVideo = 0x99,
            /// <summary>
            /// Nihon Bussan
            /// </summary>
            NihonBussan = 0x9A,
            /// <summary>
            /// Tecmo
            /// </summary>
            Tecmo = 0x9B,
            /// <summary>
            /// Imagineer
            /// </summary>
            Imagineer = 0x9C,
            /// <summary>
            /// Face
            /// </summary>
            Face = 0x9E,
            /// <summary>
            /// Scorpion Soft
            /// </summary>
            ScorpionSoft = 0xA2,
            /// <summary>
            /// Broderbund
            /// </summary>
            Broderbund = 0xA3,
            /// <summary>
            /// Konami
            /// </summary>
            Konami = 0xA4,
            /// <summary>
            /// K. Amusement Leasing Co. (KAC)
            /// </summary>
            KAmusementLeasingCO = 0xA5,
            /// <summary>
            /// Kawada Co., Ltd.
            /// </summary>
            KawadaCOLtd = 0xA6,
            /// <summary>
            /// Takara
            /// </summary>
            Takara = 0xA7,
            /// <summary>
            /// Royal Industries
            /// </summary>
            RoyalIndustries = 0xA8,
            /// <summary>
            /// Tecnos
            /// </summary>
            Tecnos = 0xA9,
            /// <summary>
            /// Victor Musical Industries
            /// </summary>
            VictorMusicalIndustries = 0xAA,
            /// <summary>
            /// Hi-Score Media Work
            /// </summary>
            HIScoreMediaWork = 0xAB,
            /// <summary>
            /// Toei Animation
            /// </summary>
            ToeiAnimation = 0xAC,
            /// <summary>
            /// Toho (Japan)
            /// </summary>
            TohoJap = 0xAD,
            /// <summary>
            /// TSS
            /// </summary>
            Tss = 0xAE,
            /// <summary>
            /// Namco
            /// </summary>
            Namco = 0xAF,
            /// <summary>
            /// Acclaim (Japan)
            /// </summary>
            AcclaimJap = 0xB0,
            /// <summary>
            /// ASCII Corporation / Nexoft
            /// </summary>
            AsciiCorporationNexoft = 0xB1,
            /// <summary>
            /// Bandai
            /// </summary>
            Bandai = 0xB2,
            /// <summary>
            /// Soft Pro Inc.
            /// </summary>
            SoftProInc = 0xB3,
            /// <summary>
            /// Enix
            /// </summary>
            Enix = 0xB4,
            /// <summary>
            /// dB-SOFT
            /// </summary>
            DBSoft = 0xB5,
            /// <summary>
            /// HAL Laboratory
            /// </summary>
            HalLaboratory = 0xB6,
            /// <summary>
            /// SNK
            /// </summary>
            Snk = 0xB7,
            /// <summary>
            /// Pony Canyon
            /// </summary>
            PonyCanyon = 0xB9,
            /// <summary>
            /// Culture Brain
            /// </summary>
            CultureBrain = 0xBA,
            /// <summary>
            /// Sunsoft
            /// </summary>
            Sunsoft = 0xBB,
            /// <summary>
            /// Toshiba EMI
            /// </summary>
            ToshibaEmi = 0xBC,
            /// <summary>
            /// CBS/Sony Group
            /// </summary>
            CbsSonyGroup = 0xBD,
            /// <summary>
            /// Sammy Corporation
            /// </summary>
            SammyCorporation = 0xBF,
            /// <summary>
            /// Taito
            /// </summary>
            Taito = 0xC0,
            /// <summary>
            /// Sunsoft / Ask Co., Ltd.
            /// </summary>
            SunsoftAskCOLtd = 0xC1,
            /// <summary>
            /// Kemco
            /// </summary>
            Kemco = 0xC2,
            /// <summary>
            /// Square / Disk Original Group (DOG)
            /// </summary>
            SquareDiskOriginalGroup = 0xC3,
            /// <summary>
            /// Tokuma Shoten
            /// </summary>
            TokumaShoten = 0xC4,
            /// <summary>
            /// Data East
            /// </summary>
            DataEast = 0xC5,
            /// <summary>
            /// Tonkin House / Tokyo Shoseki
            /// </summary>
            TonkinHouseTokyoShoseki = 0xC6,
            /// <summary>
            /// East Cube / Toho (NA)
            /// </summary>
            EastCubeTohoNA = 0xC7,
            /// <summary>
            /// Koei
            /// </summary>
            Koei = 0xC8,
            /// <summary>
            /// UPL
            /// </summary>
            Upl = 0xC9,
            /// <summary>
            /// Konami / Ultra / Palcom
            /// </summary>
            KonamiUltraPalcom = 0xCA,
            /// <summary>
            /// NTVIC / VAP
            /// </summary>
            NtvicVap = 0xCB,
            /// <summary>
            /// Use Co., Ltd.
            /// </summary>
            UseCOLtd = 0xCC,
            /// <summary>
            /// Meldac
            /// </summary>
            Meldac = 0xCD,
            /// <summary>
            /// Pony Canyon / FCI
            /// </summary>
            PonyCanyonFci = 0xCE,
            /// <summary>
            /// Angel
            /// </summary>
            Angel = 0xCF,
            /// <summary>
            /// Disco
            /// </summary>
            Disco = 0xD0,
            /// <summary>
            /// Sofel
            /// </summary>
            Sofel = 0xD1,
            /// <summary>
            /// Bothtec, Inc. / Quest
            /// </summary>
            BothtecIncQuest = 0xD2,
            /// <summary>
            /// Sigma Enterprises
            /// </summary>
            SigmaEnterprises = 0xD3,
            /// <summary>
            /// Ask Corp.
            /// </summary>
            AskCorp = 0xD4,
            /// <summary>
            /// Kyugo Trading Co.
            /// </summary>
            KyugoTradingCO = 0xD5,
            /// <summary>
            /// Naxat Soft / Kaga Tech
            /// </summary>
            NaxatSoftKagaTech = 0xD6,
            /// <summary>
            /// Status
            /// </summary>
            Status = 0xD8,
            /// <summary>
            /// Banpresto
            /// </summary>
            Banpresto = 0xD9,
            /// <summary>
            /// Tomy
            /// </summary>
            Tomy = 0xDA,
            /// <summary>
            /// Hiro Co., Ltd.
            /// </summary>
            HiroCOLtd = 0xDB,
            /// <summary>
            /// Nippon Computer Systems (NCS) / Masaya Games
            /// </summary>
            NipponComputerSystemsMasayaGames = 0xDD,
            /// <summary>
            /// Human Creative
            /// </summary>
            HumanCreative = 0xDE,
            /// <summary>
            /// Altron
            /// </summary>
            Altron = 0xDF,
            /// <summary>
            /// K.K. DCE
            /// </summary>
            KKDce = 0xE0,
            /// <summary>
            /// Towa Chiki
            /// </summary>
            TowaChiki = 0xE1,
            /// <summary>
            /// Yutaka
            /// </summary>
            Yutaka = 0xE2,
            /// <summary>
            /// Kaken Corporation
            /// </summary>
            KakenCorporation = 0xE3,
            /// <summary>
            /// Epoch
            /// </summary>
            Epoch = 0xE5,
            /// <summary>
            /// Athena
            /// </summary>
            Athena = 0xE7,
            /// <summary>
            /// Asmik
            /// </summary>
            Asmik = 0xE8,
            /// <summary>
            /// Natsume
            /// </summary>
            Natsume = 0xE9,
            /// <summary>
            /// King Records
            /// </summary>
            KingRecords = 0xEA,
            /// <summary>
            /// Atlus
            /// </summary>
            Atlus = 0xEB,
            /// <summary>
            /// Sony Music Entertainment
            /// </summary>
            SonyMusicEntertainment = 0xEC,
            /// <summary>
            /// Pixel Corporation
            /// </summary>
            PixelCorporation = 0xED,
            /// <summary>
            /// Information Global Service (IGS)
            /// </summary>
            InformationGlobalService = 0xEE,
            /// <summary>
            /// Fujimic
            /// </summary>
            Fujimic = 0xEF,
            /// <summary>
            /// A-Wave
            /// </summary>
            AWave = 0xF0,
        }

        [MarshalAs(UnmanagedType.U1)]
        // Raw byte: 0x01
        private readonly byte blockType = 1;
        /// <summary>
        /// Valid block type ID
        /// </summary>
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
        /// Manufacturer code. 0x00 = Unlicensed, 0x01 = Nintendo
        /// </summary>
        public Company LicenseeCode { get => (Company)manufacturerCode; set => manufacturerCode = (byte)value; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        byte[] gameName = Encoding.ASCII.GetBytes("---");
        /// <summary>
        /// 3-letter ASCII code per game (e.g. ZEL for The Legend of Zelda)
        /// </summary>
        public string GameName { get => Encoding.ASCII.GetString(gameName).Trim(new char[] { '\0', ' ' }); set => gameName = Encoding.ASCII.GetBytes(value.PadRight(3)).Take(3).ToArray(); }

        [MarshalAs(UnmanagedType.U1)]
        byte gameType;
        /// <summary>
        /// 0x20 = " " — Normal disk
        /// 0x45 = "E" — Event(e.g.Japanese national DiskFax tournaments)
        /// 0x52 = "R" — Reduction in price via advertising
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
        /// Disk type. 0x00 = FMC ("normal card"), 0x01 = FSC ("card with shutter"). May correlate with FMC and FSC product codes
        /// </summary>
        public DiskTypes DiskType { get => (DiskTypes)diskType; set => diskType = (byte)value; }

        [MarshalAs(UnmanagedType.U1)]
        // Speculative: (Err.10) Possibly indicates disk #; usually $00
        // Speculative: 0x00 = yellow disk, 0x01 = blue or gold disk, 0xFE = white disk, 0xFF = blue disk
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
        byte[] manufacturingDate = { 0, 0, 0 };
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
        // 0x49 = Japan
        byte countryCode = (byte)Country.Japan;
        /// <summary>
        /// Country code. 0x49 = Japan
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
        readonly byte[] unknown10 = { 0, 0, 0, 0, 0 };

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        byte[] rewrittenDate = { 0, 0, 0 };
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
        /// Disk rewrite count. 0x00 = Original (no copies)
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

        /// <summary>
        /// Length of the block
        /// </summary>
        public uint Length { get => 56; }

        /// <summary>
        /// Create FdsBlockDiskInfo object from raw data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="offset">Data offset</param>
        /// <returns>FdsBlockDiskInfo object</returns>
        /// <exception cref="InvalidDataException"></exception>
        public static FdsBlockDiskInfo FromBytes(byte[] data, int offset = 0)
        {
            int rawsize = Marshal.SizeOf(typeof(FdsBlockDiskInfo));
            if (rawsize > data.Length - offset)
                throw new InvalidDataException("Not enough data to fill FdsDiskInfoBlock class. Array length from position: " + (data.Length - offset) + ", struct length: " + rawsize);
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(data, offset, buffer, rawsize);
            FdsBlockDiskInfo retobj = (FdsBlockDiskInfo)Marshal.PtrToStructure(buffer, typeof(FdsBlockDiskInfo));
            Marshal.FreeHGlobal(buffer);
            return retobj;
        }

        /// <summary>
        /// Returns raw data
        /// </summary>
        /// <returns>Data</returns>
        public byte[] ToBytes()
        {
            int rawSize = Marshal.SizeOf(this);
            IntPtr buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.StructureToPtr(this, buffer, false);
            byte[] data = new byte[rawSize];
            Marshal.Copy(buffer, data, 0, rawSize);
            Marshal.FreeHGlobal(buffer);
            return data;
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Game name</returns>
        public override string ToString() => GameName;

        /// <summary>
        /// Equality comparer
        /// </summary>
        /// <param name="other">Other FdsBlockDiskInfo object</param>
        /// <returns>True if objects are equal</returns>
        public bool Equals(FdsBlockDiskInfo other)
        {
            return Enumerable.SequenceEqual(this.ToBytes(), other.ToBytes());
        }
    }
}
