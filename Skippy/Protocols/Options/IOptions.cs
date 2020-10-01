using System.Collections.Generic;

namespace Skippy.Protocols
{

    public interface IOption
    {
        string Name { get; set; }
        string Term { get; set; }
    }
    public interface IOptions : ICollection<IOption> 
    {
        object GetIncrementedValue(object currentValue);
        object GetDecrementedValue(object currentValue);

    }

}
