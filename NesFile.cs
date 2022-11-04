using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace com.clusterrr.Famicom.Containers
{
    /// <summary>
    /// iNES file container for NES/Famicom games
    /// </summary>
    public partial class NesFile
    {
        /// <summary>
        /// PRG data
        /// </summary>
        public IEnumerable<byte> PRG
        {
            get => Array.AsReadOnly(prg);
            set => prg = (value ?? new byte[0]).ToArray();
        }
        /// <summary>
        /// CHR data (can be null if none)
        /// </summary>
        public IEnumerable<byte> CHR
        {
            get => Array.AsReadOnly(chr);
            set => chr = (value ?? new byte[0]).ToArray();
        }
        /// <summary>
        /// Trainer (can be null if none)
        /// </summary>
        public IEnumerable<byte> Trainer
        {
            get => Array.AsReadOnly(trainer);
            set
            {
                if (value != null && value.Count() != 0 && value.Count() != 512)
                    throw new ArgumentOutOfRangeException("Trainer size must be 512 bytes");
                chr = (value ?? new byte[0]).ToArray();
            }
        }
        /// <summary>
        /// Miscellaneous ROM (NES 2.0 only, can be null if none)
        /// </summary>
        public IEnumerable<byte> MiscellaneousROM
        {
            get => Array.AsReadOnly(miscellaneousROM);
            set => miscellaneousROM = (value ?? new byte[0]).ToArray();
        }
        /// <summary>
        /// Mapper number
        /// </summary>
        public ushort Mapper { get; set; } = 0;
        /// <summary>
        /// Submapper number (NES 2.0 only)
        /// </summary>
        public byte Submapper { get; set; } = 0;
        /// <summary>
        /// Battery-backed (or other non-volatile memory) memory is present
        /// </summary>
        public bool Battery { get; set; } = false;
        private iNesVersion version = NesFile.iNesVersion.iNES;
        /// <summary>
        /// Version of .nes file format: iNES or NES 2.0
        /// </summary>
        public iNesVersion Version
        {
            get => version;
            set
            {
                if (value != iNesVersion.iNES && value != iNesVersion.NES20)
                    throw new ArgumentException("Only version 1 and 2 allowed", nameof(Version));
                version = value;
            }
        }
        /// <summary>
        /// PRG RAM Size (NES 2.0 only)
        /// </summary>
        public uint PrgRamSize { get; set; } = 0;
        private uint prgNvRamSize = 0;
        /// <summary>
        /// PRG NVRAM Size (NES 2.0 only)
        /// </summary>
        public uint PrgNvRamSize
        {
            get => prgNvRamSize; set
            {
                prgNvRamSize = value;
                if (prgNvRamSize > 0 || chrNvRamSize > 0)
                    Battery = true;
            }
        }
        /// <summary>
        /// CHR RAM Size (NES 2.0 only)
        /// </summary>
        public uint ChrRamSize { get; set; } = 0;
        private uint chrNvRamSize = 0;
        private byte[] prg = new byte[0];
        private byte[] chr = new byte[0];
        private byte[] trainer = new byte[0];
        private byte[] miscellaneousROM = new byte[0];

        /// <summary>
        /// CHR NVRAM Size (NES 2.0 only)
        /// </summary>
        public uint ChrNvRamSize
        {
            get => chrNvRamSize; set
            {
                chrNvRamSize = value;
                if (prgNvRamSize > 0 || chrNvRamSize > 0)
                    Battery = true;
            }
        }
        /// <summary>
        /// Mirroring type
        /// </summary>
        public MirroringType Mirroring { get; set; } = MirroringType.Horizontal;
        /// <summary>
        /// For non-homebrew NES/Famicom games, this field's value is always a function of the region in which a game was released (NES 2.0 only)
        /// </summary>        
        public Timing Region { get; set; } = Timing.Ntsc;
        /// <summary>
        /// Console type (NES 2.0 only)
        /// </summary>
        public ConsoleType Console { get; set; } = ConsoleType.Normal;
        /// <summary>
        /// Vs. System PPU type (used when Console is ConsoleType.VsSystem)
        /// </summary>
        public VsPpuType VsPpu { get; set; } = VsPpuType.RP2C03B;
        /// <summary>
        /// Vs. System hardware type (used when Console is ConsoleType.VsSystem)
        /// </summary>
        public VsHardwareType VsHardware { get; set; } = VsHardwareType.VsUnisystemNormal;
        /// <summary>
        /// Extended console type (used when Console is ConsoleType.Extended)
        /// </summary>
        public ExtendedConsoleType ExtendedConsole { get; set; } = ExtendedConsoleType.RegularNES;
        /// <summary>
        /// Default expansion device (NES 2.0 only)
        /// </summary>
        public ExpansionDevice DefaultExpansionDevice { get; set; } = ExpansionDevice.Unspecified;
        /// <summary>
        /// Miscellaneous ROMs сount (NES 2.0 only)
        /// </summary>
        public byte MiscellaneousROMsCount { get; set; } = 0;

        /// <summary>
        /// Version of iNES format
        /// </summary>
        public enum iNesVersion
        {
            /// <summary>
            /// Classic iNES format
            /// </summary>
            iNES = 1,
            /// <summary>
            /// NES 2.0 format
            /// </summary>
            NES20 = 2
        }
        /// <summary>
        /// Timing type, depends on region
        /// </summary>
        public enum Timing
        {
            /// <summary>
            /// NTSC, RP2C02, North America, Japan, South Korea, Taiwan
            /// </summary>
            Ntsc = 0,
            /// <summary>
            /// PAL, RP2C07, Western Europe, Australia
            /// </summary>
            Pal = 1,
            /// <summary>
            /// Used either if a game was released with identical ROM content in both NTSC and PAL countries, such as Nintendo's early games, or if the game detects the console's timing and adjusts itself
            /// </summary>
            Multiple = 2,
            /// <summary>
            /// Dendy, UMC 6527P and clones, Eastern Europe, Russia, Mainland China, India, Africa
            /// </summary>
            Dendy = 3
        };

        /// <summary>
        /// Console type
        /// </summary>
        public enum ConsoleType
        {
            /// <summary>
            /// Nintendo Entertainment System/Family Computer
            /// </summary>
            Normal = 0,
            /// <summary>
            /// Nintendo Vs. System
            /// </summary>
            VsSystem = 1,
            /// <summary>
            /// Nintendo Playchoice 10
            /// </summary>
            Playchoice10 = 2,
            /// <summary>
            /// Extended Console Type
            /// </summary>
            Extended = 3
        }

        /// <summary>
        /// Vs. System PPU type
        /// </summary>
        public enum VsPpuType
        {
            /// <summary>
            /// RP2C03B
            /// </summary>
            RP2C03B = 0x00,
            /// <summary>
            /// RP2C03G
            /// </summary>
            RP2C03G = 0x01,
            /// <summary>
            /// RP2C04-0001
            /// </summary>
            RP2C04_0001 = 0x02,
            /// <summary>
            /// RP2C04-0002
            /// </summary>            
            RP2C04_0002 = 0x03,
            /// <summary>
            /// RP2C04-0003
            /// </summary>
            RP2C04_0003 = 0x04,
            /// <summary>
            /// RP2C04-0004
            /// </summary>
            RP2C04_0004 = 0x05,
            /// <summary>
            /// RC2C03B
            /// </summary>
            RC2C03B = 0x06,
            /// <summary>
            /// RC2C03C
            /// </summary>
            RC2C03C = 0x07,
            /// <summary>
            /// RC2C05-01 ($2002 AND $?? =$1B)
            /// </summary>
            RC2C05_01 = 0x08,
            /// <summary>
            /// RC2C05-02 ($2002 AND $3F =$3D)
            /// </summary>
            RC2C05_02 = 0x09,
            /// <summary>
            /// RC2C05-03 ($2002 AND $1F =$1C)
            /// </summary>
            RC2C05_03 = 0x0A,
            /// <summary>
            /// RC2C05-04 ($2002 AND $1F =$1B)
            /// </summary>
            RC2C05_04 = 0x0B,
            /// <summary>
            /// RC2C05-05 ($2002 AND $1F =unknown)
            /// </summary>
            RC2C05_05 = 0x0C
        };

        /// <summary>
        /// Vs. System hardware type
        /// </summary>
        public enum VsHardwareType
        {
            /// <summary>
            /// Vs. Unisystem (normal)
            /// </summary>
            VsUnisystemNormal = 0x00,
            /// <summary>
            /// Vs. Unisystem (RBI Baseball protection)
            /// </summary>
            VsUnisystemRBIBaseballProtection = 0x01,
            /// <summary>
            /// Vs. Unisystem (TKO Boxing protection)
            /// </summary>
            VsUnisystemTKOBoxingProtection = 0x02,
            /// <summary>
            /// Vs. Unisystem (Super Xevious protection)
            /// </summary>
            VsUnisystemSuperXeviousProtection = 0x03,
            /// <summary>
            /// Vs. Unisystem (Vs. Ice Climber Japan protection)
            /// </summary>
            VsUnisystemVsIceClimberJapanProtection = 0x04,
            /// <summary>
            /// Vs. Dual System (normal)
            /// </summary>
            VsDualSystemNormal = 0x05,
            /// <summary>
            /// Vs. Dual System (Raid on Bungeling Bay protection)
            /// </summary>
            VsDualSystemRaidOnBungelingBayProtection = 0x06
        }

        /// <summary>
        /// Extended console type
        /// </summary>
        public enum ExtendedConsoleType
        {
            /// <summary>
            /// Regular NES/Famicom/Dendy
            /// </summary>
            RegularNES = 0x00,
            /// <summary>
            /// Nintendo Vs. System
            /// </summary>
            NintendoVsSystem = 0x01,
            /// <summary>
            /// Playchoice 10
            /// </summary>
            Playchoice10 = 0x02,
            /// <summary>
            /// Regular Famiclone, but with CPU that supports Decimal Mode (e.g. Bit Corporation Creator)
            /// </summary>
            FamicloneWithDecimalMode = 0x03,
            /// <summary>
            /// V.R. Technology VT01 with monochrome palette
            /// </summary>
            VRTechnologyVT01Monochrome = 0x04,
            /// <summary>
            /// V.R. Technology VT01 with red/cyan STN palette
            /// </summary>
            VRTechnologyVT01WithRedCyanSTNPalette = 0x05,
            /// <summary>
            /// V.R. Technology VT02
            /// </summary>
            VRTechnologyVT02 = 0x06,
            /// <summary>
            /// V.R. Technology VT03
            /// </summary>
            VRTechnologyVT03 = 0x07,
            /// <summary>
            /// V.R. Technology VT09
            /// </summary>
            VRTechnologyVT09 = 0x08,
            /// <summary>
            /// V.R. Technology VT32
            /// </summary>
            VRTechnologyVT32 = 0x09,
            /// <summary>
            /// V.R. Technology VT369
            /// </summary>
            VRTechnologyVT369 = 0x0A,
            /// <summary>
            /// UMC UM6578
            /// </summary>
            UMC_UM6578 = 0x0B
        }

        /// <summary>
        /// Type of expansion device connected to console, source: https://www.nesdev.org/wiki/NES_2.0#Default_Expansion_Device
        /// </summary>
        public enum ExpansionDevice
        {
            /// <summary>
            /// Expansion device is not specified
            /// </summary>
            Unspecified = 0x00,
            /// <summary>
            /// Standard NES/Famicom controllers
            /// </summary>
            Standard = 0x01,
            /// <summary>
            /// NES Four Score/Satellite with two additional standard controllers
            /// </summary>
            NesFourScore = 0x02,
            /// <summary>
            /// Famicom Four Players Adapter with two additional standard controllers
            /// </summary>
            FamicomFourPlayersAdapter = 0x03,
            /// <summary>
            /// Vs. System
            /// </summary>
            VsSystem = 0x04,
            /// <summary>
            /// Vs. System with reversed inputs
            /// </summary>
            VsSystemWithReversedInputs = 0x05,
            /// <summary>
            /// Vs. Pinball (Japan)
            /// </summary>
            VsPinball = 0x06,
            /// <summary>
            /// Vs. Zapper
            /// </summary>
            VsZapper = 0x07,
            /// <summary>
            /// Zapper ($4017)
            /// </summary>
            Zapper = 0x08,
            /// <summary>
            /// Two Zappers
            /// </summary>
            TwoZappers = 0x09,
            /// <summary>
            /// Bandai Hyper Shot Lightgun
            /// </summary>
            BandaiHyperShotLightgun = 0x0A,
            /// <summary>
            /// Power Pad Side A
            /// </summary>
            PowerPadSideA = 0x0B,
            /// <summary>
            /// Power Pad Side B
            /// </summary>
            PowerPadSideB = 0x0C,
            /// <summary>
            /// Family Trainer Side A
            /// </summary>
            FamilyTrainerSideA = 0x0D,
            /// <summary>
            /// Family Trainer Side B
            /// </summary>
            FamilyTrainerSideB = 0x0E,
            /// <summary>
            /// Arkanoid Vaus Controller (NES)
            /// </summary>
            ArkanoidVausControllerNES = 0x0F,
            /// <summary>
            /// Arkanoid Vaus Controller (Famicom)
            /// </summary>
            ArkanoidVausControllerFamicom = 0x10,
            /// <summary>
            /// Two Vaus Controllers plus Famicom Data Recorder
            /// </summary>
            TwoVausControllersPlusFamicomDataRecorder = 0x11,
            /// <summary>
            /// Konami Hyper Shot Controller
            /// </summary>
            KonamiHyperShotController = 0x12,
            /// <summary>
            /// Coconuts Pachinko Controller
            /// </summary>
            CoconutsPachinkoController = 0x13,
            /// <summary>
            /// Exciting Boxing Punching Bag (Blowup Doll)
            /// </summary>
            ExcitingBoxingPunchingBag = 0x14,
            /// <summary>
            /// Jissen Mahjong Controller
            /// </summary>
            JissenMahjongController = 0x15,
            /// <summary>
            /// Party Tap
            /// </summary>
            PartyTap = 0x16,
            /// <summary>
            /// Oeka Kids Tablet
            /// </summary>
            OekaKidsTablet = 0x17,
            /// <summary>
            /// Sunsoft Barcode Battler
            /// </summary>
            SunsoftBarcodeBattler = 0x18,
            /// <summary>
            /// Miracle Piano Keyboard
            /// </summary>
            MiraclePianoKeyboard = 0x19,
            /// <summary>
            /// Pokkun Moguraa (Whack-a-Mole Mat and Mallet)
            /// </summary>
            PokkunMoguraa = 0x1A,
            /// <summary>
            /// Top Rider(Inflatable Bicycle)
            /// </summary>
            TopRider = 0x1B,
            /// <summary>
            /// Double-Fisted (Requires or allows use of two controllers by one player)
            /// </summary>
            DoubleFisted = 0x1C,
            /// <summary>
            /// Famicom 3D System
            /// </summary>
            Famicom3DSystem = 0x1D,
            /// <summary>
            /// Doremikko Keyboard
            /// </summary>
            DoremikkoKeyboard = 0x1E,
            /// <summary>
            /// R.O.B. Gyro Set
            /// </summary>
            RobGyroSet = 0x1F,
            /// <summary>
            /// Famicom Data Recorder (don't emulate keyboard)
            /// </summary>
            FamicomDataRecorder = 0x20,
            /// <summary>
            /// ASCII Turbo File
            /// </summary>
            ASCIITurboFile = 0x21,
            /// <summary>
            /// IGS Storage Battle Box
            /// </summary>
            IGSStorageBattleBox = 0x22,
            /// <summary>
            /// Family BASIC Keyboard plus Famicom Data Recorder
            /// </summary>
            FamilyBasicKeyboardPlusFamicomDataRecorder = 0x23,
            /// <summary>
            /// Dongda PEC-586 Keyboard
            /// </summary>
            DongdaPEC586Keyboard = 0x24,
            /// <summary>
            /// Bit Corp. Bit-79 Keyboard
            /// </summary>
            BitCorpBit79Keyboard = 0x25,
            /// <summary>
            /// Subor Keyboard
            /// </summary>
            SuborKeyboard = 0x26,
            /// <summary>
            /// Subor Keyboard plus mouse (3x8-bit protocol)
            /// </summary>
            SuborKeyboardPlusMouse3x8 = 0x27,
            /// <summary>
            /// Subor Keyboard plus mouse (24-bit protocol)
            /// </summary>
            SuborKeyboardPlusMouse24 = 0x28,
            /// <summary>
            /// SNES Mouse ($4017.d0)
            /// </summary>
            SnesMouse4017 = 0x29,
            /// <summary>
            /// Multicart
            /// </summary>
            Multicart = 0x2A,
            /// <summary>
            /// Two SNES controllers replacing the two standard NES controllers
            /// </summary>
            TwoSnesControllers = 0x2B,
            /// <summary>
            /// RacerMate Bicycle
            /// </summary>
            RacerMateBicycle = 0x2C,
            /// <summary>
            /// U-Force
            /// </summary>
            UForce = 0x2D,
            /// <summary>
            /// R.O.B. Stack-Up
            /// </summary>
            RobStackUp = 0x2E,
            /// <summary>
            /// City Patrolman Lightgun
            /// </summary>
            CityPatrolmanLightgun = 0x2F,
            /// <summary>
            /// Sharp C1 Cassette Interface
            /// </summary>
            SharpC1CassetteInterface = 0x30,
            /// <summary>
            /// Standard Controller with swapped Left-Right/Up-Down/B-A
            /// </summary>
            StandardControllerWithSwapped = 0x31,
            /// <summary>
            /// Excalibor Sudoku Pad
            /// </summary>
            ExcaliborSudokuPad = 0x32,
            /// <summary>
            /// ABL Pinball
            /// </summary>
            AblPinball = 0x33,
            /// <summary>
            /// Golden Nugget Casino extra buttons
            /// </summary>
            GoldenNuggetCasinoExtraButtons = 0x34,
        }

        /// <summary>
        /// Constructor to create empty NesFile object
        /// </summary>
        public NesFile()
        {
        }

        /// <summary>
        /// Create NesFile object from raw .nes file contents
        /// </summary>
        /// <param name="data">Raw .nes file data</param>
        public NesFile(byte[] data)
        {
            var header = new byte[16];
            Array.Copy(data, header, header.Length);
            if (header[0] != 'N' ||
                header[1] != 'E' ||
                header[2] != 'S' ||
                header[3] != 0x1A) throw new InvalidDataException("Invalid iNES header");

            if ((header[7] & 0x0C) == 0x08)
                Version = iNesVersion.NES20;
            else if (!(header[12] == 0 && header[13] == 0 && header[14] == 0 && header[15] == 0))
            {
                // archaic iNES
                header[7] = header[8] = header[9] = header[10] = header[11] = header[12] = header[13] = header[14] = header[15] = 0;
            }

            uint prgSize = 0;
            uint chrSize = 0;
            Mirroring = (MirroringType)(header[6] & 1);
            Battery = (header[6] & (1 << 1)) != 0;
            if ((header[6] & (1 << 2)) != 0)
                trainer = new byte[512];
            else
                trainer = new byte[0];
            if ((header[6] & (1 << 3)) != 0)
                Mirroring = MirroringType.FourScreenVram;
            if (Version == iNesVersion.iNES)
            {
                prgSize = (uint)(header[4] * 0x4000);
                chrSize = (uint)(header[5] * 0x2000);
                Mapper = (byte)((header[6] >> 4) | (header[7] & 0xF0));
                Console = (ConsoleType)(header[7] & 3);
                PrgRamSize = (uint)(header[8] == 0 ? 0x2000 : header[8] * 0x2000);
            }
            else if (Version == iNesVersion.NES20) // NES 2.0
            {
                if ((header[9] & 0x0F) != 0x0F)
                    prgSize = (uint)((((header[9] & 0x0F) << 8) | header[4]) * 0x4000);
                else
                    prgSize = (uint)((1 << (header[4] >> 2)) * ((header[4] & 3) * 2 + 1)); // omg
                if ((header[9] & 0xF0) != 0xF0)
                    chrSize = (uint)((((header[9] & 0xF0) << 4) | header[5]) * 0x2000);
                else
                    chrSize = (uint)((1 << (header[5] >> 2)) * ((header[5] & 3) * 2 + 1));
                Mapper = (ushort)((header[6] >> 4) | (header[7] & 0xF0) | ((header[8] & 0x0F) << 8));
                Submapper = (byte)(header[8] >> 4);
                Console = (ConsoleType)(header[7] & 3);
                if ((header[10] & 0x0F) > 0)
                    PrgRamSize = (uint)(64 << (header[10] & 0x0F));
                if ((header[10] & 0xF0) > 0)
                    PrgNvRamSize = (uint)(64 << ((header[10] & 0xF0) >> 4));
                if ((header[11] & 0x0F) > 0)
                    ChrRamSize = (uint)(64 << (header[11] & 0x0F));
                if ((header[11] & 0xF0) > 0)
                    ChrNvRamSize = (uint)(64 << ((header[11] & 0xF0) >> 4));
                Region = (Timing)header[12];
                switch (Console)
                {
                    case ConsoleType.VsSystem:
                        VsPpu = (VsPpuType)(header[13] & 0x0F);
                        VsHardware = (VsHardwareType)(header[13] >> 4);
                        break;
                    case ConsoleType.Extended:
                        ExtendedConsole = (ExtendedConsoleType)(header[13] & 0x0F);
                        break;
                }
                MiscellaneousROMsCount = (byte)(header[14] & 3);
                DefaultExpansionDevice = (ExpansionDevice)(header[15] & 0x3F);
            }

            uint offset = (uint)header.Length;
            if (trainer != null && trainer.Length > 0)
            {
                if (offset < data.Length)
                    Array.Copy(data, offset, trainer, 0, Math.Max(0, Math.Min(trainer.Length, data.Length - offset)));
                offset += (uint)trainer.Length;
            }

            prg = new byte[prgSize];
            if (offset < data.Length)
                Array.Copy(data, offset, prg, 0, Math.Max(0, Math.Min(prgSize, data.Length - offset))); // Ignore end for some bad ROMs
            offset += prgSize;

            chr = new byte[chrSize];
            if (offset < data.Length)
                Array.Copy(data, offset, chr, 0, Math.Max(0, Math.Min(chrSize, data.Length - offset)));
            offset += chrSize;

            if (MiscellaneousROMsCount > 0)
            {
                MiscellaneousROM = new byte[data.Length - offset];
                Array.Copy(data, offset, miscellaneousROM, 0, miscellaneousROM.Length);
            }
            else
            {
                MiscellaneousROM = new byte[0];
            }
        }

        /// <summary>
        /// Create NesFile object from the specified .nes file 
        /// </summary>
        /// <param name="fileName">Path to the .nes file</param>
        public NesFile(string fileName)
            : this(File.ReadAllBytes(fileName))
        {
        }

        /// <summary>
        /// Create NesFile object from raw .nes file contents
        /// </summary>
        /// <param name="data">Raw ROM data</param>
        /// <returns>NesFile object</returns>
        public static NesFile FromBytes(byte[] data) => new NesFile(data);

        /// <summary>
        /// Create NesFile object from the specified .nes file 
        /// </summary>
        /// <param name="filename">Path to the .nes file</param>
        /// <returns>NesFile object</returns>
        public static NesFile FromFile(string filename) => new NesFile(filename);

        /// <summary>
        /// Returns .nes file contents (header + PRG + CHR)
        /// </summary>
        /// <returns>.nes file contents</returns>
        public byte[] ToBytes()
        {
            var data = new List<byte>();
            var header = new byte[16];
            header[0] = (byte)'N';
            header[1] = (byte)'E';
            header[2] = (byte)'S';
            header[3] = 0x1A;
            if (prg == null) prg = new byte[0];
            if (chr == null) chr = new byte[0];
            if (trainer == null) trainer = new byte[0];
            ulong prgSizePadded, chrSizePadded;
            if (Version == iNesVersion.iNES)
            {
                if (Console == ConsoleType.Extended)
                    throw new InvalidDataException("Extended console type is supported by NES 2.0 only");
                if (Mapper > 255)
                    throw new InvalidDataException("Mapper number > 255 is supported by NES 2.0 only");
                if (Submapper != 0)
                    throw new InvalidDataException("Submapper number is supported by NES 2.0 only");
                var length16k = prg.Length / 0x4000;
                if (length16k > 0xFF) throw new ArgumentOutOfRangeException("PRG size is too big for iNES, use NES 2.0 instead");
                header[4] = (byte)Math.Ceiling((double)prg.Length / 0x4000);
                prgSizePadded = header[4] * 0x4000UL;
                var length8k = chr.Length / 0x2000;
                if (length8k > 0xFF) throw new ArgumentOutOfRangeException("CHR size is too big for iNES, use NES 2.0 instead");
                header[5] = (byte)Math.Ceiling((double)chr.Length / 0x2000);
                chrSizePadded = header[5] * 0x2000UL;
                switch (Mirroring)
                {
                    case MirroringType.Unknown:          // mirroring field ignored
                    case MirroringType.Horizontal:
                    case MirroringType.Vertical:
                    case MirroringType.FourScreenVram:
                    case MirroringType.MapperControlled: // mirroring field ignored
                        break;
                    default:
                        throw new InvalidDataException($"{Mirroring} mirroring is not supported by iNES");
                }
                // Hard-wired nametable mirroring type
                if (Mirroring == MirroringType.Vertical)
                    header[6] |= 1;
                // "Battery" and other non-volatile memory
                if (Battery)
                    header[6] |= (1 << 1);
                // 512-byte Trainer
                if (trainer.Length > 0)
                    header[6] |= (1 << 2);
                // Hard-wired four-screen mode
                if (Mirroring == MirroringType.FourScreenVram)
                    header[6] |= (1 << 3);
                // Mapper Number D0..D3
                header[6] |= (byte)(Mapper << 4);
                // Console type
                header[7] |= (byte)((byte)Console & 3);
                // Mapper Number D4..D7
                header[7] |= (byte)(Mapper & 0xF0);

                data.AddRange(header);
                if (trainer.Length > 0)
                    data.AddRange(trainer);
                data.AddRange(prg);
                data.AddRange(chr);
            }
            else if (Version == iNesVersion.NES20)
            {
                var length16k = (uint)Math.Ceiling((double)prg.Length / 0x4000);
                if (length16k <= 0xEFF)
                {
                    header[4] = (byte)(length16k & 0xFF);
                    header[9] |= (byte)(length16k >> 8);
                    prgSizePadded = length16k * 0x4000;
                }
                else
                {
                    byte exponent, multiplier;
                    (exponent, multiplier, prgSizePadded) = SizeToExponent((ulong)prg.Length);
                    header[4] = (byte)((exponent << 2) | (multiplier & 3));
                    header[9] |= 0x0F;
                }
                var length8k = (uint)Math.Ceiling((double)chr.Length / 0x2000);
                if (length8k <= 0xEFF)
                {
                    header[5] = (byte)(length8k & 0xFF);
                    header[9] |= (byte)((length8k >> 4) & 0xF0);
                    chrSizePadded = length8k * 0x2000;
                }
                else
                {
                    byte exponent, multiplier;
                    (exponent, multiplier, chrSizePadded) = SizeToExponent((ulong)chr.Length);
                    header[5] = (byte)((exponent << 2) | (multiplier & 3));
                    header[9] |= 0xF0;
                }
                // Hard-wired nametable mirroring type
                if (Mirroring == MirroringType.Vertical)
                    header[6] |= 1;
                // "Battery" and other non-volatile memory
                if (Battery)
                    header[6] |= (1 << 1);
                // 512-byte Trainer
                if (trainer.Length > 0)
                    header[6] |= (1 << 2);
                // Hard-wired four-screen mode
                if (Mirroring == MirroringType.FourScreenVram)
                    header[6] |= (1 << 3);
                // Mapper Number D0..D3
                header[6] |= (byte)(Mapper << 4);
                // Console type
                header[7] |= (byte)((byte)Console & 3);
                // NES 2.0 identifier
                header[7] |= 1 << 3;
                // Mapper Number D4..D7
                header[7] |= (byte)(Mapper & 0xF0);
                // Mapper number D8..D11
                header[8] |= (byte)((Mapper >> 8) & 0x0F);
                // Submapper
                header[8] |= (byte)(Submapper << 4);
                // PRG RAM (volatile) shift count
                var prgRamBitSize = PrgRamSize > 0 ? Math.Max(1, (int)Math.Ceiling(Math.Log(PrgRamSize, 2)) - 6) : 0;
                header[10] |= (byte)(prgRamBitSize & 0x0F);
                // PRG-NVRAM/EEPROM (non-volatile) shift count
                var prgNvRamBitSize = PrgNvRamSize > 0 ? Math.Max(1, (int)Math.Ceiling(Math.Log(PrgNvRamSize, 2)) - 6) : 0;
                header[10] |= (byte)((prgNvRamBitSize << 4) & 0xF0);
                // CHR-RAM size (volatile) shift count
                var chrRamBitSize = ChrRamSize > 0 ? Math.Max(1, (int)Math.Ceiling(Math.Log(ChrRamSize, 2)) - 6) : 0;
                header[11] |= (byte)(chrRamBitSize & 0x0F);
                // CHR-NVRAM size (non-volatile) shift count
                var chrNvRamBitSize = ChrNvRamSize > 0 ? Math.Max(1, (int)Math.Ceiling(Math.Log(ChrNvRamSize, 2)) - 6) : 0;
                header[11] |= (byte)((chrNvRamBitSize << 4) & 0xF0);
                // CPU/PPU timing mode
                header[12] |= (byte)((byte)Region & 3);
                switch (Console)
                {
                    // When Byte 7 AND 3 =1: Vs. System Type
                    case ConsoleType.VsSystem:
                        // Vs. PPU Type
                        header[13] |= (byte)((byte)VsPpu & 0x0F);
                        // Vs. Hardware Type
                        header[13] |= (byte)(((byte)VsHardware << 4) & 0xF0);
                        break;
                    // When Byte 7 AND 3 =3: Extended Console Type
                    case ConsoleType.Extended:
                        // Extended Console Type
                        header[13] = (byte)ExtendedConsole;
                        break;
                }
                // Miscellaneous ROMs
                header[14] |= (byte)(MiscellaneousROMsCount & 3);
                // Default Expansion Device
                header[15] |= (byte)((byte)DefaultExpansionDevice & 0x3F);

                data.AddRange(header);
                if (Trainer != null)
                    data.AddRange(Trainer);
                data.AddRange(prg);
                data.AddRange(Enumerable.Repeat<byte>(0xFF, (int)prgSizePadded - prg.Length));
                data.AddRange(chr);
                data.AddRange(Enumerable.Repeat<byte>(0xFF, (int)chrSizePadded - chr.Length));
                if (MiscellaneousROMsCount > 0 || (MiscellaneousROM != null && MiscellaneousROM.Count() > 0))
                {
                    if (MiscellaneousROMsCount == 0)
                        throw new InvalidDataException("MiscellaneousROMsCount is zero while MiscellaneousROM is not empty");
                    if (MiscellaneousROM == null)
                        throw new InvalidDataException("MiscellaneousROM is empty while MiscellaneousROMsCount is not zero");
                    data.AddRange(MiscellaneousROM);
                }
            }
            return data.ToArray();
        }

        /// <summary>
        /// Save as .nes file
        /// </summary>
        /// <param name="filename">Target filename</param>
        public void Save(string filename) => File.WriteAllBytes(filename, ToBytes());

        private static ulong ExponentToSize(byte exponent, byte multiplier)
            => (1UL << exponent) * (ulong)(multiplier * 2 + 1);

        private static (byte Exponent, byte Multiplier, ulong Padded) SizeToExponent(ulong value)
        {
            if (value == 0) return (0, 0, 1);
            if (value < 8)
            {
                var r = SizeToExponent(value << 3);
                return ((byte)(r.Exponent - 3), r.Multiplier, r.Padded >> 3);
            }

            // Calculate bits required to store number
            byte bitsize = 0;
            while (value >> bitsize > 0) bitsize++;

            // Split it into two parts
            var major = value >> (bitsize - 3);
            var minor = value & (ulong)~(0b111 << (bitsize - 3));

            // Round up
            if (minor != 0) major++;

            byte e, m;
            switch (major)
            {
                case 0b100:
                    e = (byte)(bitsize - 1);
                    m = 0; // 0*2+1=1
                    break;
                case 0b101:
                    e = (byte)(bitsize - 3);
                    m = 2; // 2*2+1=5
                    break;
                case 0b110:
                    e = (byte)(bitsize - 2);
                    m = 1; // 1*2+1=3
                    break;
                case 0b111:
                    e = (byte)(bitsize - 3);
                    m = 3; // 3*2+1=7
                    break;
                case 0b1000:
                    e = (byte)bitsize;
                    m = 0; // 0*2+1=1
                    break;
                default:
                    throw new InvalidProgramException();
            }

            return (e, m, ExponentToSize(e, m));
        }

        /// <summary>
        /// Calculate MD5 checksum of ROM (CHR+PRG without header)
        /// </summary>
        public byte[] CalculateMD5()
        {
            var md5 = MD5.Create();
            if (prg == null) prg = new byte[0];
            if (chr == null) chr = new byte[0];
            var alldata = new byte[prg.Length + chr.Length];
            Array.Copy(prg, 0, alldata, 0, prg.Length);
            Array.Copy(chr, 0, alldata, prg.Length, chr.Count());
            return md5.ComputeHash(alldata);
        }

        /// <summary>
        /// Calculate CRC32 checksum of ROM (CHR+PRG without header)
        /// </summary>
        public uint CalculateCRC32()
        {
            if (prg == null) prg = new byte[0];
            if (chr == null) chr = new byte[0];
            var alldata = new byte[prg.Length + chr.Length];
            Array.Copy(prg, 0, alldata, 0, prg.Length);
            Array.Copy(chr, 0, alldata, prg.Length, chr.Length);
            return Crc32Calculator.CalculateCRC32(alldata);
        }
    }
}
