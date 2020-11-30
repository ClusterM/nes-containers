using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.Famicom.Containers
{
    public class FdsBlockFileAmount : IFdsBlock
    {
        private byte blockType = 2;
        /// <summary>
        /// True if block type ID is valid
        /// </summary>
        public bool IsValid { get => blockType == 2; }

        private byte fileAmount;
        public byte FileAmount { get => fileAmount; set => fileAmount = value; }

        /// <summary>
        /// Set by dumper. True when checksum is ok
        /// </summary>
        public bool CrcOk { get; set; } = true;

        /// <summary>
        /// Set by dumper. True when "end of head" flag was meet during dumping
        /// </summary>
        public bool EndOfHeadMeet { get; set; } = false;

        public uint Length => 2;

        public static FdsBlockFileAmount FromBytes(byte[] rawData, int position = 0)
        {
            var retobj = new FdsBlockFileAmount();
            retobj.blockType = rawData[position];
            retobj.fileAmount = rawData[position + 1];
            return retobj;
        }

        public byte[] ToBytes()
        {
            return new byte[] { blockType, fileAmount };
        }
    }
}
