namespace com.clusterrr.Famicom.Containers
{
    public interface IFdsBlock
    {
        bool IsValid { get; }
        bool CrcOk { get; set; }
        bool EndOfHeadMeet { get; set; }
        uint Length { get; }
        byte[] ToBytes();
    }
}
