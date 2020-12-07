using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.Famicom.Containers
{
    public class FdsBlockFileData : IFdsBlock
    {
        private byte blockType = 4;
        /// <summary>
        /// True if block type ID is valid
        /// </summary>
        public bool IsValid { get => blockType == 4; }

        private byte[] data = new byte[0];
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

        public uint Length => (uint)(data.Length + 1);

        public static FdsBlockFileData FromBytes(byte[] rawData, int position = 0, int size = -1)
        {
            var retobj = new FdsBlockFileData();
            retobj.blockType = rawData[position];
            retobj.data = new byte[size < 0 ? rawData.Length - position - 1 : size - 1];
            Array.Copy(rawData, position + 1, retobj.data, 0, retobj.data.Length);
            return retobj;
        }

        public byte[] ToBytes()
        {
            var result = new List<byte>();
            result.Add(blockType);
            result.AddRange(Data);
            return result.ToArray();
        }

        public override string ToString() => $"{data.Length} bytes";
    }
}
