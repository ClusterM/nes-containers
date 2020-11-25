using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.Famicom.Containers
{
    [StructLayout(LayoutKind.Sequential, Size = 18, Pack = 1, CharSet = CharSet.Ansi)]
    public class FdsFileHeaderBlock : IFdsBlock
    {
        public enum Kind
        {
            Program = 0, 
            Character = 1,
            NameTable = 2
        }

        [MarshalAs(UnmanagedType.U1)]
        private byte blockType = 3;

        [MarshalAs(UnmanagedType.U1)]
        private byte fileNumber;
        public byte FileNumber { get => fileNumber; set => fileNumber = value; }

        [MarshalAs(UnmanagedType.U1)]
        // ID specified at disk-read function call
        private byte fileIndicateCode;
        public byte FileIndicateCode { get => fileIndicateCode; set => fileIndicateCode = value; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private char[] fileName;
        public string FileName { get => new string(fileName).Trim(new char[] { '\0', ' ' }); set => fileName = value.PadRight(0).ToCharArray(); }

        [MarshalAs(UnmanagedType.U2)]
        // the destination address when loading
        private ushort fileAddress;
        public ushort FileAddress { get => fileAddress; set => fileAddress = value; }

        [MarshalAs(UnmanagedType.U2)]
        private ushort fileSize;
        public ushort FileSize { get => fileSize; set => fileSize = value; }

        [MarshalAs(UnmanagedType.U1)]
        private byte fileKind;
        public Kind FileKind { get => (Kind)fileKind; set => fileKind = (byte)value; }

        [MarshalAs(UnmanagedType.U1)]
        private bool crcOk;
        public bool CrcOk { get => crcOk; set => crcOk = value; }

        [MarshalAs(UnmanagedType.U1)]
        private bool endOfHeadMeet;
        public bool EndOfHeadMeet { get => endOfHeadMeet; set => endOfHeadMeet = value; }

        public static FdsFileHeaderBlock FromBytes(byte[] rawData, int position = 0)
        {
            int rawsize = Marshal.SizeOf(typeof(FdsFileHeaderBlock));
            if (rawsize > rawData.Length - position)
            {
                if (rawsize <= rawData.Length - position + 2)
                {
                    var newRawData = new byte[rawsize];
                    Array.Copy(rawData, position, newRawData, 0, rawsize - 2);
                    rawData = newRawData;
                    position = 0;
                }
                else
                {
                    throw new ArgumentException("Not enough data to fill FdsFileHeaderBlock class. Array length from position: " + (rawData.Length - position) + ", Struct length: " + rawsize);
                }
            }
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(rawData, position, buffer, rawsize);
            FdsFileHeaderBlock retobj = (FdsFileHeaderBlock)Marshal.PtrToStructure(buffer, typeof(FdsFileHeaderBlock));
            Marshal.FreeHGlobal(buffer);
            if (retobj.blockType != 3)
                throw new InvalidDataException("Invalid block type");
            return retobj;
        }

        public byte[] ToBytes()
        {
            int rawSize = Marshal.SizeOf(this);
            IntPtr buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.StructureToPtr(this, buffer, false);
            byte[] rawDatas = new byte[rawSize - 2];
            Marshal.Copy(buffer, rawDatas, 0, rawSize - 2);
            Marshal.FreeHGlobal(buffer);
            return rawDatas;
        }
    }
}
