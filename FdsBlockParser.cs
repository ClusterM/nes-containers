using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.clusterrr.Famicom.Containers
{
    public static class FdsBlockParser
    {
        public static IFdsBlock FromBytes(byte[] data)
        {
            // Check block type
            switch (data[0])
            {
                case 0: return FdsBlockDiskInfo.FromBytes(data);
                case 1: return FdsBlockFileAmount.FromBytes(data);
                case 2: return FdsBlockFileHeader.FromBytes(data);
                case 3: return FdsBlockFileData.FromBytes(data);
                default: throw new InvalidDataException("Invalid FDS block type");
            }
        }
    }
}
