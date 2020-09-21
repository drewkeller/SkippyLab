using System.Threading.Tasks;

namespace Skippy
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Use this to indicate no awaiting, which suppresses warnings when not awaiting async methods.
        /// </summary>
        /// <param name="task"></param>
        public static void NoAwait(this Task task) { }

    }
}
