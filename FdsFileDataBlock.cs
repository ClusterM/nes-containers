using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.Famicom.Containers
{
    public class FdsFileDataBlock : IFdsBlock
    {
        private byte blockType = 4;

        private byte[] data = new byte[0];
        public IEnumerable<byte> Data
        {
            get => Array.AsReadOnly(data);
            set => data = value.ToArray();
        }

        public bool CrcOk { get; set; }

        public bool EndOfHeadMeet { get; set; }

        public static FdsFileDataBlock FromBytes(byte[] rawData, int position = 0, int size = -1)
        {
            var retobj = new FdsFileDataBlock();
            retobj.blockType = rawData[position];
            if (retobj.blockType != 4)
                throw new InvalidDataException("Invalid block type");
            retobj.data = new byte[size < 0 ? rawData.Length - position : size];
            Array.Copy(rawData, position, retobj.data, 0, retobj.data.Length);
            return retobj;
        }

        public byte[] ToBytes()
        {
            var result = new List<byte>();
            result.Add(blockType);
            result.AddRange(Data);
            return result.ToArray();
        }
    }
}
