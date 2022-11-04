namespace com.clusterrr.Famicom.Containers
{
    /// <summary>
    /// FDS block interface
    /// </summary>
    public interface IFdsBlock
    {
        /// <summary>
        /// Returns the valid block type ID
        /// </summary>
        byte ValidTypeID { get;  }
        /// <summary>
        /// Returns true if the block type ID is valid
        /// </summary>
        bool IsValid { get; }
        /// <summary>
        /// Length of raw block data
        /// </summary>
        uint Length { get; }
        /// <summary>
        /// Return raw data
        /// </summary>
        /// <returns></returns>
        byte[] ToBytes();
    }
}
