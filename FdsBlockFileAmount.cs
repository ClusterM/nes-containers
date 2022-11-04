using System;
using System.IO;
using System.Linq;

namespace com.clusterrr.Famicom.Containers
{
    /// <summary>
    /// File amount FDS block (block type 2)
    /// </summary>
    public class FdsBlockFileAmount : IFdsBlock, IEquatable<FdsBlockFileAmount>
    {
        private byte blockType = 2;
        /// <summary>
        /// Valid block type ID
        /// </summary>
        public byte ValidTypeID { get => 2; }
        /// <summary>
        /// True if block type ID is valid
        /// </summary>
        public bool IsValid { get => blockType == ValidTypeID; }

        private byte fileAmount;
        /// <summary>
        /// Amount of files
        /// </summary>
        public byte FileAmount { get => fileAmount; set => fileAmount = value; }

        /// <summary>
        /// Length of the block
        /// </summary>
        public uint Length { get => 2; }

        /// <summary>
        /// Create FdsBlockFileAmount object from raw data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="offset">Data offset</param>
        /// <returns>FdsBlockFileAmount object</returns>
        public static FdsBlockFileAmount FromBytes(byte[] data, int offset = 0)
        {
            if (data.Length - offset < 2)
                throw new InvalidDataException("Not enough data to fill FdsBlockFileAmount class. Array length from position: " + (data.Length - offset) + ", struct length: 2");
            return new FdsBlockFileAmount
            {
                blockType = data[offset],
                fileAmount = data[offset + 1]
            };
        }

        /// <summary>
        /// Return raw data
        /// </summary>
        /// <returns>Data</returns>
        public byte[] ToBytes() => new byte[] { blockType, fileAmount };

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>File amount as string</returns>
        public override string ToString() => $"{FileAmount}";

        /// <summary>
        /// Equality comparer
        /// </summary>
        /// <param name="other">Other FdsBlockFileAmount object</param>
        /// <returns>True if objects are equal</returns>
        public bool Equals(FdsBlockFileAmount other)
        {
            return Enumerable.SequenceEqual(this.ToBytes(), other.ToBytes());
        }
    }
}
