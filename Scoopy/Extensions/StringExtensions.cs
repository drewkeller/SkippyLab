namespace Scoopy.Extensions
{
    public static class StringExtensions
    {

        /// <summary>
        /// Gets whether the string is not null and not empty.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static bool HasData(this string @this)
        {
            return !string.IsNullOrEmpty(@this);
        }

    }
}
