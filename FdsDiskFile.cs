using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.Famicom.Containers
{
    public class FdsDiskFile
    {
        private FdsBlockFileHeader headerBlock;
        private FdsBlockFileData dataBlock;
        public FdsBlockFileHeader HeaderBlock { get => headerBlock; set => headerBlock = value; }
        public FdsBlockFileData DataBlock { get => dataBlock; set => dataBlock = value; }

        public byte FileNumber { get => HeaderBlock.FileNumber; set => HeaderBlock.FileNumber = value; }
        public byte FileIndicateCode { get => HeaderBlock.FileIndicateCode; set => HeaderBlock.FileIndicateCode = value; }
        public string FileName { get => HeaderBlock.FileName; set => HeaderBlock.FileName = value; }
        public ushort FileAddress { get => HeaderBlock.FileAddress; set => HeaderBlock.FileAddress = value; }
        public ushort FileSize { get => (ushort)DataBlock.Data.Count(); }
        public FdsBlockFileHeader.Kind FileKind { get => HeaderBlock.FileKind; set => HeaderBlock.FileKind = value; }
        public IEnumerable<byte> Data
        {
            get => DataBlock.Data;
            set
            {
                DataBlock.Data = value;
                HeaderBlock.FileSize = (ushort)DataBlock.Data.Count();
            }
        }

        public FdsDiskFile(FdsBlockFileHeader headerBlock, FdsBlockFileData dataBlock)
        {
            this.HeaderBlock = headerBlock;
            this.DataBlock = dataBlock;
            headerBlock.FileSize = (ushort)dataBlock.Data.Count();
        }

        public FdsDiskFile()
        {
            this.HeaderBlock = new FdsBlockFileHeader();
            this.DataBlock = new FdsBlockFileData();
            HeaderBlock.FileSize = (ushort)DataBlock.Data.Count();
        }

        public byte[] ToBytes() => Enumerable.Concat(HeaderBlock.ToBytes(), DataBlock.ToBytes()).ToArray();

        public override string ToString() => $"{FileName} ({FileKind}, {dataBlock})";
    }
}
