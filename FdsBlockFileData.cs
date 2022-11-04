using System;
using System.Collections.Generic;
using System.Linq;

namespace com.clusterrr.Famicom.Containers
{
    /// <summary>
    /// File data FDS block (block type 4)
    /// </summary>
    public class FdsBlockFileData : IFdsBlock, IEquatable<FdsBlockFileData>
    {
        private byte blockType = 4;
        /// <summary>
        /// Valid block type ID
        /// </summary>
        public byte ValidTypeID { get => 4; }
        /// <summary>
        /// True if block type ID is valid
        /// </summary>
        public bool IsValid { get => blockType == ValidTypeID; }

        private byte[] data = new byte[0];
        /// <summary>
        /// File data
        /// </summary>
        public IEnumerable<byte> Data
        {
            get => Array.AsReadOnly(data);
            set => data = value.ToArray();
        }

        /// <summary>
        /// Set by dumper. True when checksum is ok
        /// </summary>
        public bool CrcOk { get; set; } = true;

        /// <summary>
        /// Set by dumper. True when "end of head" flag was meet during dumping
        /// </summary>
        public bool EndOfHeadMeet { get; set; } = false;

        /// <summary>
        /// Length of the block
        /// </summary>
        public uint Length => (uint)(data.Length + 1);

        /// <summary>
        /// Create FdsBlockFileData object from raw data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="offset">Offset</param>
        /// <param name="length">Length</param>
        /// <returns>FdsBlockFileData object</returns>
        public static FdsBlockFileData FromBytes(byte[] data, int offset = 0, int length = -1)
        {
            var retobj = new FdsBlockFileData
            {
                blockType = data[offset],
                data = new byte[length < 0 ? data.Length - offset - 1 : length - 1]
            };
            Array.Copy(data, offset + 1, retobj.data, 0, retobj.data.Length);
            return retobj;
        }

        /// <summary>
        /// Return raw data
        /// </summary>
        /// <returns>Data</returns>
        public byte[] ToBytes()
        {
            var result = new List<byte>
            {
                blockType
            };
            result.AddRange(Data);
            return result.ToArray();
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Number of bytes as string</returns>
        public override string ToString() => $"{data.Length} bytes";

        /// <summary>
        /// Equality comparer
        /// </summary>
        /// <param name="other">Other FdsBlockFileData object</param>
        /// <returns>True if objects are equal</returns>
        public bool Equals(FdsBlockFileData other)
        {
            return Enumerable.SequenceEqual(this.ToBytes(), other.ToBytes());
        }
    }
}
