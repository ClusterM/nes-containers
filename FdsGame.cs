using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.Famicom.Containers
{
    public class FdsGame
    {
        IList<FdsDiskSide> sides;
        /// <summary>
        /// Disk Side Images
        /// </summary>
        public IList<FdsDiskSide> Sides { get => sides; set => sides = value; }

        public FdsGame()
        {
            sides = new List<FdsDiskSide>();
        }

        public FdsGame(string filename) : this(File.ReadAllBytes(filename))
        {
        }

        public FdsGame(byte[] data) : this()
        {
            if (data[0] == (byte)'F' && data[1] == (byte)'D' && data[2] == (byte)'S')
                data = data.Skip(16).ToArray(); // skip header
            for (int i = 0; i < data.Length; i += 65500)
            {
                var sideData = data.Skip(i).Take(66500).ToArray();
                sides.Add(FdsDiskSide.FromBytes(sideData));
            }
        }

        public FdsGame(IEnumerable<FdsDiskSide> sides)
        {
            this.sides = new List<FdsDiskSide>(sides);
        }

        public static FdsGame FromBytes(byte[] data)
        {
            return new FdsGame(data);
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
                header[3] = (byte)sides.Count();
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
