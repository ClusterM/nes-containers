using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.Famicom.Containers
{
    public class FdsFile
    {
        IList<FdsDiskSide> sides;
        /// <summary>
        /// Disk Side Images
        /// </summary>
        public IList<FdsDiskSide> Sides { get => sides; set => sides = value; }

        public FdsFile()
        {
            sides = new List<FdsDiskSide>();
        }

        public FdsFile(string filename) : this(File.ReadAllBytes(filename))
        {
        }

        public FdsFile(byte[] data) : this()
        {
            if (data[0] == (byte)'F' && data[1] == (byte)'D' && data[2] == (byte)'S' && data[3] == 0x1A)
                data = data.Skip(16).ToArray(); // skip header
            for (int i = 0; i < data.Length; i += 65500)
            {
                var sideData = data.Skip(i).Take(66500).ToArray();
                sides.Add(FdsDiskSide.FromBytes(sideData));
            }
        }

        public FdsFile(IEnumerable<FdsDiskSide> sides)
        {
            this.sides = new List<FdsDiskSide>(sides);
        }

        public static FdsFile FromBytes(byte[] data)
        {
            return new FdsFile(data);
        }

        public byte[] ToBytes(bool useHeader = false)
        {
            var data = sides.SelectMany(s => s.ToBytes());
            if (useHeader)
            {
                var header = new byte[16];
                header[0] = (byte)'F';
                header[1] = (byte)'D';
                header[2] = (byte)'S';
                header[3] = 0x1A;
                header[4] = (byte)sides.Count();
                data = Enumerable.Concat(header, data);
            }
            return data.ToArray();
        }

        public void Save(string filename, bool useHeader = false)
        {
            File.WriteAllBytes(filename, ToBytes(useHeader));
        }
    }
}
