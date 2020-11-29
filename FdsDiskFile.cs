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

        public byte FileNumber { get => headerBlock.FileNumber; set => headerBlock.FileNumber = value; }
        public byte FileIndicateCode { get => headerBlock.FileIndicateCode; set => headerBlock.FileIndicateCode = value; }
        public string FileName { get => headerBlock.FileName; set => headerBlock.FileName = value; }
        public ushort FileAddress { get => headerBlock.FileAddress; set => headerBlock.FileAddress = value; }
        public ushort FileSize { get => (ushort)dataBlock.Data.Count(); }
        public FdsBlockFileHeader.Kind FileKind { get => headerBlock.FileKind; set => headerBlock.FileKind = value; }
        public IEnumerable<byte> Data
        {
            get => dataBlock.Data;
            set
            {
                dataBlock.Data = value;
                headerBlock.FileSize = (ushort)dataBlock.Data.Count();
            }
        }

        public FdsDiskFile(FdsBlockFileHeader headerBlock, FdsBlockFileData dataBlock)
        {
            this.headerBlock = headerBlock;
            this.dataBlock = dataBlock;
            headerBlock.FileSize = (ushort)dataBlock.Data.Count();
        }

        public FdsDiskFile()
        {
            this.headerBlock = new FdsBlockFileHeader();
            this.dataBlock = new FdsBlockFileData();
            headerBlock.FileSize = (ushort)dataBlock.Data.Count();
        }

        public byte[] ToBytes() => Enumerable.Concat(headerBlock.ToBytes(), dataBlock.ToBytes()).ToArray();
    }
}
