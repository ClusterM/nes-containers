using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace com.clusterrr.Famicom.Containers
{
    /// <summary>
    /// Full .fds file, disk sides collection
    /// </summary>
    public class FdsFile
    {
        IList<FdsDiskSide> sides;
        /// <summary>
        /// Disk Side Images
        /// </summary>
        public IList<FdsDiskSide> Sides { get => sides; set => sides = value; }

        /// <summary>
        /// Constructor to create empty FdsFile object
        /// </summary>
        public FdsFile()
        {
            sides = new List<FdsDiskSide>();
        }

        /// <summary>
        /// Create FdsFile object from the specified .nes file 
        /// </summary>
        /// <param name="filename">Path to the .fds file</param>
        public FdsFile(string filename) : this(File.ReadAllBytes(filename))
        {
        }

        /// <summary>
        /// Create FdsFile object from raw .fds file data
        /// </summary>
        /// <param name="data"></param>
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

        /// <summary>
        /// Create FdsFile object from set of FdsDiskSide objects 
        /// </summary>
        /// <param name="sides"></param>
        public FdsFile(IEnumerable<FdsDiskSide> sides)
        {
            this.sides = new List<FdsDiskSide>(sides);
        }

        /// <summary>
        /// Create FdsFile object from raw .fds file contents
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static FdsFile FromBytes(byte[] data)
        {
            return new FdsFile(data);
        }

        /// <summary>
        /// Return FDS file contents
        /// </summary>
        /// <returns>FDS file contents</returns>
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

        /// <summary>
        /// Save to .fds file
        /// </summary>
        /// <param name="filename">Target filename</param>
        /// <param name="useHeader">Option to add .fds file header (ignored by most emulators)</param>
        public void Save(string filename, bool useHeader = false)
        {
            File.WriteAllBytes(filename, ToBytes(useHeader));
        }
    }
}
