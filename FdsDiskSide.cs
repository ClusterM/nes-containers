using System;
using System.Collections.Generic;
using System.Linq;
using static com.clusterrr.Famicom.Containers.FdsBlockDiskInfo;
using System.Runtime.InteropServices;
using System.Text;

namespace com.clusterrr.Famicom.Containers
{
    /// <summary>
    /// Single FDS disk side: disk info block, file amount block and file blocks
    /// </summary>
    public class FdsDiskSide
    {
        readonly FdsBlockDiskInfo diskInfoBlock;
        /// <summary>
        /// Disk info block
        /// </summary>
        public FdsBlockDiskInfo DiskInfoBlock { get => diskInfoBlock; }

        /// <summary>
        /// Literal ASCII string: *NINTENDO-HVC*
        /// </summary>
        public string DiskVerification => diskInfoBlock.DiskVerification;

        /// <summary>
        /// Manufacturer code. 0x00 = Unlicensed, 0x01 = Nintendo
        /// </summary>
        public Company LicenseeCode { get => diskInfoBlock.LicenseeCode; set => diskInfoBlock.LicenseeCode = value; }

        /// <summary>
        /// 3-letter ASCII code per game (e.g. ZEL for The Legend of Zelda)
        /// </summary>
        public string? GameName { get => diskInfoBlock.GameName; set => diskInfoBlock.GameName = value; }

        /// <summary>
        /// 0x20 = " " — Normal disk
        /// 0x45 = "E" — Event(e.g.Japanese national DiskFax tournaments)
        /// 0x52 = "R" — Reduction in price via advertising
        /// </summary>
        public char GameType { get => diskInfoBlock.GameType; set => diskInfoBlock.GameType = value; }

        /// <summary>
        /// Game version/revision number. Starts at 0x00, increments per revision
        /// </summary>
        public byte GameVersion { get => diskInfoBlock.GameVersion; set => diskInfoBlock.GameVersion = value; }

        /// <summary>
        /// Side number. Single-sided disks use A
        /// </summary>
        public DiskSides DiskSide { get => diskInfoBlock.DiskSide; set => diskInfoBlock.DiskSide = value; }

        /// <summary>
        /// Disk number. First disk is 0x00, second is 0x01, etc.
        /// </summary>
        public byte DiskNumber { get => diskInfoBlock.DiskNumber; set => diskInfoBlock.DiskNumber = value; }

        /// <summary>
        /// Disk type. 0x00 = FMC ("normal card"), 0x01 = FSC ("card with shutter"). May correlate with FMC and FSC product codes
        /// </summary>
        public DiskTypes DiskType { get => diskInfoBlock.DiskType; set => diskInfoBlock.DiskType = value; }

        /// <summary>
        /// Unknown, offset 0x18.
        /// Always 0x00
        /// </summary>
        public byte Unknown01 { get => diskInfoBlock.Unknown01; set => diskInfoBlock.Unknown01 = value; }

        /// <summary>
        /// Boot read file code. Refers to the file code/file number to load upon boot/start-up
        /// </summary>
        public byte BootFile { get => diskInfoBlock.BootFile; set => diskInfoBlock.BootFile = value; }

        /// <summary>
        /// Unknown, offset 0x1A.
        /// Always 0xFF
        /// </summary>
        public byte Unknown02 { get => diskInfoBlock.Unknown02; set => diskInfoBlock.Unknown02 = value; }

        /// <summary>
        /// Unknown, offset 0x1B.
        /// Always 0xFF
        /// </summary>
        public byte Unknown03 { get => diskInfoBlock.Unknown03; set => diskInfoBlock.Unknown03 = value; }

        /// <summary>
        /// Unknown, offset 0x1C.
        /// Always 0xFF
        /// </summary>
        public byte Unknown04 { get => diskInfoBlock.Unknown04; set => diskInfoBlock.Unknown04 = value; }

        /// <summary>
        /// Unknown, offset 0x1D.
        /// Always 0xFF
        /// </summary>
        public byte Unknown05 { get => diskInfoBlock.Unknown05; set => diskInfoBlock.Unknown05 = value; }

        /// <summary>
        /// Unknown, offset 0x1E.
        /// Always 0xFF
        /// </summary>
        public byte Unknown06 { get => diskInfoBlock.Unknown06; set => diskInfoBlock.Unknown06 = value; }

        /// <summary>
        /// Manufacturing date
        /// </summary>
        public DateTime? ManufacturingDate { get => diskInfoBlock.ManufacturingDate; set => diskInfoBlock.ManufacturingDate = value; }

        /// <summary>
        /// Country code. 0x49 = Japan
        /// </summary>
        public Country CountryCode { get => diskInfoBlock.CountryCode; set => diskInfoBlock.CountryCode = value; }

        /// <summary>
        /// Unknown, offset 0x23.
        /// Always 0x61.
        /// Speculative: Region code?
        /// </summary>
        public byte Unknown07 { get => diskInfoBlock.Unknown07; set => diskInfoBlock.Unknown07 = value; }

        /// <summary>
        /// Unknown, offset 0x24.
        /// Always 0x00.
        /// Speculative: Location/site?
        /// </summary>
        public byte Unknown08 { get => diskInfoBlock.Unknown08; set => diskInfoBlock.Unknown08 = value; }

        /// <summary>
        /// Unknown, offset 0x25.
        /// Always 0x00
        /// </summary>
        public byte Unknown09 { get => diskInfoBlock.Unknown09; set => diskInfoBlock.Unknown09 = value; }

        /// <summary>
        /// Unknown, offset 0x26.
        /// Always 0x02
        /// </summary>
        public byte Unknown10 { get => diskInfoBlock.Unknown10; set => diskInfoBlock.Unknown10 = value; }

        /// <summary>
        /// Unknown, offset 0x27. Speculative: some kind of game information representation?
        /// </summary>
        public byte Unknown11 { get => diskInfoBlock.Unknown11; set => diskInfoBlock.Unknown11 = value; }

        /// <summary>
        /// Unknown, offset 0x28. Speculative: some kind of game information representation?
        /// </summary>
        public byte Unknown12 { get => diskInfoBlock.Unknown12; set => diskInfoBlock.Unknown12 = value; }

        /// <summary>
        /// Unknown, offset 0x29. Speculative: some kind of game information representation?
        /// </summary>
        public byte Unknown13 { get => diskInfoBlock.Unknown13; set => diskInfoBlock.Unknown13 = value; }

        /// <summary>
        /// Unknown, offset 0x2A. Speculative: some kind of game information representation?
        /// </summary>
        public byte Unknown14 { get => diskInfoBlock.Unknown14; set => diskInfoBlock.Unknown14 = value; }

        /// <summary>
        /// Unknown, offset 0x2B. Speculative: some kind of game information representation?
        /// </summary>
        public byte Unknown15 { get => diskInfoBlock.Unknown15; set => diskInfoBlock.Unknown15 = value; }

        /// <summary>
        /// "Rewritten disk" date. It's speculated this refers to the date the disk was formatted and rewritten by something like a Disk Writer kiosk.
        /// In the case of an original (non-copied) disk, this should be the same as Manufacturing date
        /// </summary>
        public DateTime? RewrittenDate { get => diskInfoBlock.RewrittenDate; set => diskInfoBlock.RewrittenDate = value; }

        /// <summary>
        /// Unknown, offset 0x2F
        /// </summary>
        public byte Unknown16 { get => diskInfoBlock.Unknown16; set => diskInfoBlock.Unknown16 = value; }

        /// <summary>
        /// Unknown, offset 0x30.
        /// Always 0x80
        /// </summary>
        public byte Unknown17 { get => diskInfoBlock.Unknown17; set => diskInfoBlock.Unknown17 = value; }

        /// <summary>
        /// Disk Writer serial number
        /// </summary>
        public ushort DiskWriterSerialNumber { get => diskInfoBlock.DiskWriterSerialNumber; set => diskInfoBlock.DiskWriterSerialNumber = value; }

        /// <summary>
        /// Unknown, offset 0x33, unknown.
        /// Always 0x07
        /// </summary>
        public byte Unknown18 { get => diskInfoBlock.Unknown18; set => diskInfoBlock.Unknown18 = value; }

        /// <summary>
        /// Disk rewrite count.
        /// 0x00 = Original (no copies)
        /// </summary>
        public byte DiskRewriteCount { get => diskInfoBlock.DiskRewriteCount; set => diskInfoBlock.DiskRewriteCount = value; }

        /// <summary>
        /// Actual disk side
        /// </summary>
        public DiskSides ActualDiskSide { get => diskInfoBlock.ActualDiskSide; set => diskInfoBlock.ActualDiskSide = value; }

        /// <summary>
        /// Disk type (other)
        /// </summary>
        public DiskTypesOther DiskTypeOther { get => diskInfoBlock.DiskTypeOther; set => diskInfoBlock.DiskTypeOther = value; }

        /// <summary>
        /// Price code (deprecated, no backing)
        /// </summary>
        [Obsolete("Use \"DiskVersion\" property")]
        public byte Price { get => diskInfoBlock.DiskVersion; set => diskInfoBlock.DiskVersion = value; }

        /// <summary>
        /// Unknown how this differs from GameVersion. Disk version numbers indicate different software revisions.
        /// Speculation is that disk version incremented with each disk received from a licensee
        /// </summary>
        public byte DiskVersion { get => diskInfoBlock.DiskVersion; set => diskInfoBlock.DiskVersion = value; }

        readonly FdsBlockFileAmount fileAmountBlock;
        /// <summary>
        /// Non-hidden file amount
        /// </summary>
        public byte FileAmount { get => fileAmountBlock.FileAmount; set => fileAmountBlock.FileAmount = value; }

        readonly IList<FdsDiskFile> files;
        /// <summary>
        /// Files on disk
        /// </summary>
        public IList<FdsDiskFile> Files { get => files; }

        /// <summary>
        /// Constructor to create empty FdsDiskSide object
        /// </summary>
        public FdsDiskSide()
        {
            diskInfoBlock = new FdsBlockDiskInfo();
            fileAmountBlock = new FdsBlockFileAmount();
            files = new List<FdsDiskFile>();
        }

        /// <summary>
        /// Constructor to create FdsDiskSide object from blocks and files
        /// </summary>
        /// <param name="diskInfoBlock">Disk info block</param>
        /// <param name="fileAmountBlock">File amount block</param>
        /// <param name="files">Files</param>
        public FdsDiskSide(FdsBlockDiskInfo diskInfoBlock, FdsBlockFileAmount fileAmountBlock, IEnumerable<FdsDiskFile> files)
        {
            this.diskInfoBlock = diskInfoBlock;
            this.fileAmountBlock = fileAmountBlock;
            this.files = files.ToList();
        }

        /// <summary>
        /// Constructor to create FdsDiskSide object from blocks
        /// </summary>
        /// <param name="blocks"></param>
        public FdsDiskSide(IEnumerable<IFdsBlock> blocks)
        {
            this.diskInfoBlock = (FdsBlockDiskInfo)blocks.First();
            this.fileAmountBlock = (FdsBlockFileAmount)blocks.Skip(1).First();
            files = new List<FdsDiskFile>();
            var fileBlocks = blocks.Skip(2).ToArray();
            for (int i = 0; i < fileBlocks.Length / 2; i++)
            {
                files.Add(new FdsDiskFile((FdsBlockFileHeader)fileBlocks[i * 2], (FdsBlockFileData)fileBlocks[i * 2 + 1]));
            }
        }

        /// <summary>
        /// Constructor to create FdsDiskSide object from raw data
        /// </summary>
        /// <param name="data"></param>
        public FdsDiskSide(byte[] data) : this()
        {
            int pos = 0;
            this.diskInfoBlock = FdsBlockDiskInfo.FromBytes(data.Take(56).ToArray());
            pos += 56;
            this.fileAmountBlock = FdsBlockFileAmount.FromBytes(data.Skip(pos).Take(2).ToArray());
            pos += 2;
            while (pos < data.Length)
            {
                try
                {
                    var fileHeaderBlock = FdsBlockFileHeader.FromBytes(data.Skip(pos).Take(16).ToArray());
                    if (!fileHeaderBlock.IsValid)
                        break;
                    pos += 16;
                    var fileDataBlock = FdsBlockFileData.FromBytes(data.Skip(pos).Take(fileHeaderBlock.FileSize + 1).ToArray());
                    if (!fileDataBlock.IsValid)
                        break;
                    pos += fileHeaderBlock.FileSize + 1;
                    files.Add(new FdsDiskFile(fileHeaderBlock, fileDataBlock));
                }
                catch
                {
                    // just break on out of range
                    break;
                }
            }
        }

        /// <summary>
        /// Change file's "file number" fields orderly
        /// </summary>
        public void FixFileNumbers()
        {
            for (var i = 0; i < files.Count; i++)
                files[i].FileNumber = (byte)i;
        }

        /// <summary>
        /// Get FDS blocks
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IFdsBlock> GetBlocks()
        {
            var blocks = new List<IFdsBlock>
            {
                diskInfoBlock,
                fileAmountBlock
            };
            blocks.AddRange(files.SelectMany(f => new IFdsBlock[] { f.HeaderBlock, f.DataBlock }));
            return blocks;
        }

        /// <summary>
        /// Create FdsDiskSide object from raw data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>FdsDiskSide object</returns>

        public static FdsDiskSide FromBytes(byte[] data)
        {
            return new FdsDiskSide(data);
        }

        /// <summary>
        /// Return raw data
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            var data = Enumerable.Concat(Enumerable.Concat(diskInfoBlock.ToBytes(), fileAmountBlock.ToBytes()), files.SelectMany(f => f.ToBytes())).ToArray();
            return Enumerable.Concat(data, new byte[65500 - data.Count()]).ToArray();
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Game name, disk number, side number as string</returns>
        public override string ToString() => $"{GameName ?? "---"} - disk {DiskNumber + 1}, side {DiskSide}";
    }
}
