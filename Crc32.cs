using System;
using System.Linq;
using System.Security.Cryptography;

namespace com.clusterrr.Famicom.Containers
{
    /// <summary>
    /// CRC32 calculator
    /// </summary>
    internal class Crc32 : HashAlgorithm
    {
        private readonly uint[] table = new uint[256];
        private uint crc = 0xFFFFFFFF;

        public Crc32()
        {
            // Calculate table
            uint poly = 0xedb88320;
            uint temp;
            for (uint i = 0; i < table.Length; ++i)
            {
                temp = i;
                for (int j = 8; j > 0; --j)
                {
                    if ((temp & 1) == 1)
                    {
                        temp = (uint)((temp >> 1) ^ poly);
                    }
                    else
                    {
                        temp >>= 1;
                    }
                }
                table[i] = temp;
            }
        }

        public override void Initialize()
        {
        }

        public override bool CanReuseTransform => false;

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            while (cbSize > 0)
            {
                var pos = ibStart;
                byte index = (byte)(((crc) & 0xff) ^ array[pos]);
                crc = (crc >> 8) ^ table[index];
                ibStart++;
                cbSize--;
            }
        }

        protected override byte[] HashFinal() => BitConverter.GetBytes(~crc).Reverse().ToArray();
        public override bool CanTransformMultipleBlocks => true;
        public override byte[] Hash => BitConverter.GetBytes(~crc).Reverse().ToArray();
        public override int HashSize => 32;

    }
}
