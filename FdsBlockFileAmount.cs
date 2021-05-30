using System;
using System.Linq;

namespace com.clusterrr.Famicom.Containers
{
    public class FdsBlockFileAmount : IFdsBlock, IEquatable<FdsBlockFileAmount>
    {
        private byte blockType = 2;
        public byte ValidTypeID { get => 2; }
        /// <summary>
        /// True if block type ID is valid
        /// </summary>
        public bool IsValid { get => blockType == ValidTypeID; }

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
            var retobj = new FdsBlockFileAmount
            {
                blockType = rawData[position],
                fileAmount = rawData[position + 1]
            };
            return retobj;
        }

        public byte[] ToBytes() => new byte[] { blockType, fileAmount };

        public override string ToString() => $"{FileAmount}";

        public bool Equals(FdsBlockFileAmount other)
        {
            return Enumerable.SequenceEqual(this.ToBytes(), other.ToBytes());
        }
    }
}
