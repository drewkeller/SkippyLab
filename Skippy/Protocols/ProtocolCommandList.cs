using System.Collections.Generic;

namespace Skippy.Protocols
{
    public interface IProtocolCommandList
    {
    }

    public class ProtocolCommandList : List<IProtocolCommand>, IProtocolCommandList
    {
    }

}
