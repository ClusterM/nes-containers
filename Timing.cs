namespace com.clusterrr.Famicom.Containers
{
    /// <summary>
    /// Timing type, depends on region
    /// </summary>
    public enum Timing
    {
        /// <summary>
        /// NTSC, RP2C02, North America, Japan, South Korea, Taiwan
        /// </summary>
        Ntsc = 0,
        /// <summary>
        /// PAL, RP2C07, Western Europe, Australia
        /// </summary>
        Pal = 1,
        /// <summary>
        /// Used either if a game was released with identical ROM content in both NTSC and PAL countries, such as Nintendo's early games, or if the game detects the console's timing and adjusts itself
        /// </summary>
        Multiple = 2,
        /// <summary>
        /// Dendy, UMC 6527P and clones, Eastern Europe, Russia, Mainland China, India, Africa
        /// </summary>
        Dendy = 3
    };
}
