using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.Famicom.Containers
{
    public class FdsDiskSide
    {
        FdsBlockDiskInfo diskInfoBlock;

        public string DiskVerification { get => diskInfoBlock.DiskVerification; }
        /// <summary>
        /// Manufacturer code. $00 = Unlicensed, $01 = Nintendo
        /// </summary>
        public byte ManufacturerCode { get => diskInfoBlock.ManufacturerCode; set => diskInfoBlock.ManufacturerCode = value; }
        /// <summary>
        /// 3-letter ASCII code per game (e.g. ZEL for The Legend of Zelda)
        /// </summary>
        public string GameName { get => diskInfoBlock.GameName; set => diskInfoBlock.GameName = value; }
        /// <summary>
        /// $20 = " " — Normal disk
        /// $45 = "E" — Event(e.g.Japanese national DiskFax tournaments)
        /// $52 = "R" — Reduction in price via advertising
        /// </summary>
        public char GameType { get => diskInfoBlock.GameType; set => diskInfoBlock.GameType = value; }
        /// <summary>
        /// Game version/revision number. Starts at $00, increments per revision
        /// </summary>
        public byte GameVersion { get => diskInfoBlock.GameVersion; set => diskInfoBlock.GameVersion = value; }
        /// <summary>
        /// Side number. Single-sided disks use A
        /// </summary>
        public FdsBlockDiskInfo.DiskSides DiskSide { get => diskInfoBlock.DiskSide; set => diskInfoBlock.DiskSide = value; }
        /// <summary>
        /// Disk number. First disk is $00, second is $01, etc.
        /// </summary>
        public byte DiskNumber { get => diskInfoBlock.DiskNumber; set => diskInfoBlock.DiskNumber = value; }
        /// <summary>
        /// Disk type. $00 = FMC ("normal card"), $01 = FSC ("card with shutter"). May correlate with FMC and FSC product codes
        /// </summary>
        public FdsBlockDiskInfo.DiskTypes DiskType { get => diskInfoBlock.DiskType; set => diskInfoBlock.DiskType = value; }
        /// <summary>
        /// Boot read file code. Refers to the file code/file number to load upon boot/start-up
        /// </summary>
        public byte BootFile { get => diskInfoBlock.BootFile; set => diskInfoBlock.BootFile = value; }
        /// <summary>
        /// Manufacturing date
        /// </summary>
        public DateTime ManufacturingDate { get => diskInfoBlock.ManufacturingDate; set => diskInfoBlock.ManufacturingDate = value; }
        /// <summary>
        /// Country code. $49 = Japan
        /// </summary>
        public FdsBlockDiskInfo.Country CountryCode { get => diskInfoBlock.CountryCode; set => diskInfoBlock.CountryCode = value; }
        /// <summary>
        /// "Rewritten disk" date. It's speculated this refers to the date the disk was formatted and rewritten by something like a Disk Writer kiosk.
        /// In the case of an original (non-copied) disk, this should be the same as Manufacturing date
        /// </summary>
        public DateTime RewrittenDate { get => diskInfoBlock.RewrittenDate; set => diskInfoBlock.RewrittenDate = value; }
        /// <summary>
        /// Disk Writer serial number
        /// </summary>
        public ushort DiskWriterSerialNumber { get => diskInfoBlock.DiskWriterSerialNumber; set => diskInfoBlock.DiskWriterSerialNumber = value; }
        /// <summary>
        /// Disk rewrite count. $00 = Original (no copies)
        /// </summary>
        public byte DiskRewriteCount { get => diskInfoBlock.DiskRewriteCount; set => diskInfoBlock.DiskRewriteCount = value; }
        /// <summary>
        /// Actual disk side
        /// </summary>
        public FdsBlockDiskInfo.DiskSides ActualDiskSide { get => diskInfoBlock.ActualDiskSide; set => diskInfoBlock.ActualDiskSide = value; }
        /// <summary>
        /// Price code
        /// </summary>
        public byte Price { get => diskInfoBlock.Price; set => diskInfoBlock.Price = value; }

        FdsBlockFileAmount fileAmountBlock;
        /// <summary>
        /// Non-hidden file amount
        /// </summary>
        public byte FileAmount { get => fileAmountBlock.FileAmount; set => fileAmountBlock.FileAmount = value; }

        IList<FdsFile> files;

        /// <summary>
        /// Files on disk
        /// </summary>
        public IList<FdsFile> Files { get => files; }

        public FdsDiskSide()
        {
            diskInfoBlock = new FdsBlockDiskInfo();
            fileAmountBlock = new FdsBlockFileAmount();
            files = new List<FdsFile>();
        }

        public FdsDiskSide(FdsBlockDiskInfo diskInfoBlock, FdsBlockFileAmount fileAmountBlock, IEnumerable<FdsFile> files)
        {
            this.diskInfoBlock = diskInfoBlock;
            this.fileAmountBlock = fileAmountBlock;
            this.files = files.ToList();
            //if (this.fileAmountBlock.FileAmount > this.files.Count)
            //    throw new ArgumentOutOfRangeException("visibleFileAmount", "visibleFileAmount must be less or equal number of files");
        }

        public FdsDiskSide(IEnumerable<IFdsBlock> blocks)
        {
            this.diskInfoBlock = (FdsBlockDiskInfo)blocks.First();
            this.fileAmountBlock = (FdsBlockFileAmount)blocks.Skip(1).First();
            files = new List<FdsFile>();
            var fileBlocks = blocks.Skip(2).ToArray();
            for (int i = 0; i < fileBlocks.Length / 2; i++)
            {
                files.Add(new FdsFile((FdsBlockFileHeader)fileBlocks[i * 2], (FdsBlockFileData)fileBlocks[i * 2 + 1]));
            }
        }

        public FdsDiskSide(byte[] data) : this()
        {
            int pos = 0;
            this.diskInfoBlock = FdsBlockDiskInfo.FromBytes(data.Take(56).ToArray());
            pos += 56;
            this.fileAmountBlock = FdsBlockFileAmount.FromBytes(data.Skip(pos).Take(2).ToArray());
            pos += 2;
            while (pos < data.Length)
            {
                var fileHeaderBlock = FdsBlockFileHeader.FromBytes(data.Skip(pos).Take(16).ToArray());
                if (!fileHeaderBlock.IsValid)
                    break;
                pos += 16;
                var fileDataBlock = FdsBlockFileData.FromBytes(data.Skip(pos).Take(fileHeaderBlock.FileSize + 1).ToArray());
                if (!fileDataBlock.IsValid)
                    break;
                pos += fileHeaderBlock.FileSize + 1;
                files.Add(new FdsFile(fileHeaderBlock, fileDataBlock));
            }
        }

        public void FixFileNumbers()
        {
            for (var i = 0; i < files.Count; i++)
                files[i].FileNumber = (byte)i;
        }

        public static FdsDiskSide FromBytes(byte[] data)
        {
            return new FdsDiskSide(data);
        }

        public byte[] ToBytes()
        {
            var data = Enumerable.Concat(Enumerable.Concat(diskInfoBlock.ToBytes(), fileAmountBlock.ToBytes()), files.SelectMany(f => f.ToBytes())).ToArray();
            return Enumerable.Concat(data, new byte[65500 - data.Count()]).ToArray();
        }
    }
}
