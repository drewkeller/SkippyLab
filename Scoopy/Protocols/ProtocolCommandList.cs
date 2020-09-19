using System.Collections.Generic;

namespace Scoopy.Protocols
{
    public interface IProtocolCommandList
    {
    }

    public class ProtocolCommandList : List<IProtocolCommand>, IProtocolCommandList
    {
    }

}
