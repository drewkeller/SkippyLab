using System.Collections.Generic;

namespace Scoopy.Protocols
{

    public interface IOption
    {
        string Name { get; set; }
        string Term { get; set; }
    }
    public interface IOptions : ICollection<IOption> { }

}
