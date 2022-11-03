namespace com.clusterrr.Famicom.Containers
{
    public static class Crc32Calculator
    {
        static readonly uint[] table = new uint[256];

        static Crc32Calculator()
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

        /// <summary>
        /// Calculate CRC32 checksum
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="offset">Data offset</param>
        /// <param name="length">Length</param>
        /// <returns>CRC32 checksum</returns>
        public static uint CalculateCRC32(byte[] data, int offset, int length)
        {
            // Calculate CRC32
            uint crc = 0xffffffff;
            for (int i = offset; length > 0; i++, length--)
            {
                byte index = (byte)(((crc) & 0xff) ^ data[i]);
                crc = (crc >> 8) ^ table[index];
            }
            return ~crc;
        }

        /// <summary>
        /// Calculate CRC32 checksum
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>CRC32 checksum</returns>
        public static uint CalculateCRC32(byte[] data)
            => CalculateCRC32(data, 0, data.Length);
    }
}
