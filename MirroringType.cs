namespace com.clusterrr.Famicom.Containers
{
    /// <summary>
    /// What CIRAM A10 is connected to
    /// </summary>
    public enum MirroringType
    {
        /// <summary>
        /// PPU A11 (horizontal mirroring)
        /// </summary>
        Horizontal = 0,
        /// <summary>
        /// PPU A10 (vertical mirroring)
        /// </summary>
        Vertical = 1,
        /// <summary>
        /// Ground (one-screen A)
        /// </summary>
        OneScreenA = 2,
        /// <summary>
        /// Vcc (one-screen B)
        /// </summary>
        OneScreenB = 3,
        /// <summary>
        /// Extra memory has been added (four-screen)
        /// </summary>
        FourScreenVram = 4,
        /// <summary>
        /// Mapper controlled
        /// </summary>
        MapperControlled = 5, // for UNIF
        /// <summary>
        /// Unknown value
        /// </summary>
        Unknown = 0xff
    };
}
