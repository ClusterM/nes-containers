using System.Collections.Generic;
using System.Linq;

namespace com.clusterrr.Famicom.Containers
{
    /// <summary>
    /// FDS file - header and data
    /// </summary>
    public class FdsDiskFile
    {
        private FdsBlockFileHeader headerBlock;
        private FdsBlockFileData dataBlock;
        /// <summary>
        /// FDS block with file header
        /// </summary>
        public FdsBlockFileHeader HeaderBlock { get => headerBlock; set => headerBlock = value; }
        /// <summary>
        /// FDS block with file contents
        /// </summary>
        public FdsBlockFileData DataBlock { get => dataBlock; set => dataBlock = value; }

        /// <summary>
        /// File number
        /// </summary>
        public byte FileNumber { get => HeaderBlock.FileNumber; set => HeaderBlock.FileNumber = value; }
        /// <summary>
        /// File indicate code (ID specified at disk-read function call)
        /// </summary>
        public byte FileIndicateCode { get => HeaderBlock.FileIndicateCode; set => HeaderBlock.FileIndicateCode = value; }
        /// <summary>
        /// Filename
        /// </summary>
        public string FileName { get => HeaderBlock.FileName; set => HeaderBlock.FileName = value; }
        /// <summary>
        /// File address - the destination address when loading
        /// </summary>
        public ushort FileAddress { get => HeaderBlock.FileAddress; set => HeaderBlock.FileAddress = value; }
        /// <summary>
        /// File size
        /// </summary>
        public ushort FileSize { get => (ushort)DataBlock.Data.Count(); }
        /// <summary>
        /// Kind of the file: program, character or nametable
        /// </summary>
        public FdsBlockFileHeader.Kind FileKind { get => HeaderBlock.FileKind; set => HeaderBlock.FileKind = value; }
        /// <summary>
        /// File contents
        /// </summary>
        public IEnumerable<byte> Data
        {
            get => DataBlock.Data;
            set
            {
                DataBlock.Data = value;
                HeaderBlock.FileSize = (ushort)DataBlock.Data.Count();
            }
        }
        /// <summary>
        /// Construcor
        /// </summary>
        /// <param name="headerBlock">File header block</param>
        /// <param name="dataBlock">File data block</param>
        public FdsDiskFile(FdsBlockFileHeader headerBlock, FdsBlockFileData dataBlock)
        {
            this.headerBlock = headerBlock;
            this.dataBlock = dataBlock;
            headerBlock.FileSize = (ushort)dataBlock.Data.Count();
        }

        /// <summary>
        /// Construcor for empty FdsDiskFile object
        /// </summary>
        public FdsDiskFile()
        {
            this.headerBlock = new FdsBlockFileHeader();
            this.dataBlock = new FdsBlockFileData();
            HeaderBlock.FileSize = (ushort)DataBlock.Data.Count();
        }

        /// <summary>
        /// Returns raw file contents
        /// </summary>
        /// <returns>Raw file contents</returns>
        public byte[] ToBytes() => Enumerable.Concat(HeaderBlock.ToBytes(), DataBlock.ToBytes()).ToArray();

        /// <summary>
        /// String representation: filename, file kind, data block info
        /// </summary>
        /// <returns>String representation of the file</returns>
        public override string ToString() => $"{FileName} ({FileKind}, {dataBlock})";
    }
}
