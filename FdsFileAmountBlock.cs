using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.Famicom.Containers
{
    public class FdsFileAmountBlock : IFdsBlock
    {
        private byte blockType = 2;

        private byte fileAmount;
        public byte FileAmount { get => fileAmount; set => value = fileAmount; }

        public bool CrcOk { get; set; }

        public bool EndOfHeadMeet { get; set; }

        public static FdsFileAmountBlock FromBytes(byte[] rawData, int position = 0)
        {
            var retobj = new FdsFileAmountBlock();
            retobj.blockType = rawData[position];
            if (retobj.blockType != 2)
                throw new InvalidDataException("Invalid block type");
            retobj.fileAmount = rawData[position + 1];
            return retobj;
        }

        public byte[] ToBytes()
        {
            return new byte[] { blockType, fileAmount };
        }
    }
}
