namespace com.clusterrr.Famicom.Containers
{
    public interface IFdsBlock
    {
        byte ValidTypeID { get;  }
        bool IsValid { get; }
        uint Length { get; }
        byte[] ToBytes();
    }
}
