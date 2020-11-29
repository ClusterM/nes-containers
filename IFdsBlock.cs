using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace com.clusterrr.Famicom.Containers
{
    public interface IFdsBlock
    {
        bool IsValid { get; }
        bool CrcOk { get; set; }
        bool EndOfHeadMeet { get; set; }
        byte[] ToBytes();
    }
}
