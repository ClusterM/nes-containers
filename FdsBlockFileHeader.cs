using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace com.clusterrr.Famicom.Containers
{
    /// <summary>
    /// File header FDS block (block type 3)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 16, Pack = 1)]
    public class FdsBlockFileHeader : IFdsBlock, IEquatable<FdsBlockFileHeader>
    {
        static Encoding textEncoding = Encoding.GetEncoding("ISO-8859-1");

        /// <summary>
        /// Kind of the file
        /// </summary>
        public enum Kind
        {
            /// <summary>
            /// PRG data
            /// </summary>
            Program = 0,
            /// <summary>
            /// CHR data
            /// </summary>
            Character = 1,
            /// <summary>
            /// Nametable data
            /// </summary>
            NameTable = 2
        }

        [MarshalAs(UnmanagedType.U1)]
        private readonly byte blockType = 3;
        /// <summary>
        /// Valid block type ID
        /// </summary>
        public byte ValidTypeID { get => 3; }
        /// <summary>
        /// True if block type ID is valid
        /// </summary>
        public bool IsValid { get => blockType == 3; }

        [MarshalAs(UnmanagedType.U1)]
        private byte fileNumber;
        /// <summary>
        /// File number
        /// </summary>
        public byte FileNumber { get => fileNumber; set => fileNumber = value; }

        [MarshalAs(UnmanagedType.U1)]
        private byte fileIndicateCode;
        /// <summary>
        /// File indicate code (ID specified at disk-read function call)
        /// </summary>
        public byte FileIndicateCode { get => fileIndicateCode; set => fileIndicateCode = value; }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private byte[] fileName = textEncoding.GetBytes("FILENAME");
        /// <summary>
        /// Filename
        /// </summary>
        public string FileName
        {
            get => textEncoding.GetString(fileName).TrimEnd(new char[] { '\0' }); set
            {
                if (value.Length > 8) throw new InvalidDataException($"Filename \"{value}\" too long, must be <= 8");
                fileName = textEncoding.GetBytes(value.PadRight(8, '\0')).Take(8).ToArray();
            }
        }

        [MarshalAs(UnmanagedType.U2)]
        // the destination address when loading
        private ushort fileAddress;
        /// <summary>
        /// File address - the destination address when loading
        /// </summary>
        public ushort FileAddress { get => fileAddress; set => fileAddress = value; }

        [MarshalAs(UnmanagedType.U2)]
        private ushort fileSize;
        /// <summary>
        /// File size
        /// </summary>
        public ushort FileSize { get => fileSize; set => fileSize = value; }

        [MarshalAs(UnmanagedType.U1)]
        private byte fileKind;
        /// <summary>
        /// Kind of the file: program, character or nametable
        /// </summary>
        public Kind FileKind { get => (Kind)fileKind; set => fileKind = (byte)value; }

        /// <summary>
        /// Length of the block
        /// </summary>
        public uint Length { get => 16; }

        /// <summary>
        /// Create FdsBlockFileHeader object from raw data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="offset">Offset</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public static FdsBlockFileHeader FromBytes(byte[] data, int offset = 0)
        {
            int rawsize = Marshal.SizeOf(typeof(FdsBlockFileHeader));
            if (rawsize > data.Length - offset)
            {
                if (rawsize <= data.Length - offset + 2)
                {
                    var newRawData = new byte[rawsize];
                    Array.Copy(data, offset, newRawData, 0, rawsize - 2);
                    data = newRawData;
                    offset = 0;
                }
                else
                {
                    throw new InvalidDataException("Not enough data to fill FdsFileHeaderBlock class. Array length from position: " + (data.Length - offset) + ", struct length: " + rawsize);
                }
            }
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(data, offset, buffer, rawsize);
            FdsBlockFileHeader retobj = (FdsBlockFileHeader)Marshal.PtrToStructure(buffer, typeof(FdsBlockFileHeader));
            Marshal.FreeHGlobal(buffer);
            return retobj;
        }

        /// <summary>
        /// Returns raw data
        /// </summary>
        /// <returns>Data</returns>
        public byte[] ToBytes()
        {
            int rawSize = Marshal.SizeOf(this);
            IntPtr buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.StructureToPtr(this, buffer, false);
            byte[] data = new byte[rawSize];
            Marshal.Copy(buffer, data, 0, rawSize);
            Marshal.FreeHGlobal(buffer);
            return data;
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>File name and file kind as string</returns>
        public override string ToString() => $"{FileName} ({FileKind})";

        /// <summary>
        /// Equality comparer
        /// </summary>
        /// <param name="other">Other FdsBlockFileHeader object</param>
        /// <returns>True if objects are equal</returns>
        public bool Equals(FdsBlockFileHeader other)
        {
            return Enumerable.SequenceEqual(this.ToBytes(), other.ToBytes());
        }
    }
}
