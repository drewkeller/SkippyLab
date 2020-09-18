/// Use this utility to determine directories that are accessible on this platform.
/// https://www.mallibone.com/post/xamarin-forms-and-the-open-question-of-where-to-store-your-files
/// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Scoopy.UWP.Utilities
{
    internal class DirectoryFinder
    {
        public static void ListAvailableDirectories()
        {
            var finder = new DirectoryFinder();
            foreach (var dir in finder.DirectoryDescriptions())
            {
                // even though it finds a path here, doesn't mean we can write to it
                // also, some paths that it says don't exist, we can actually write to
                if (Directory.Exists(dir.Path))
                {
                    System.Diagnostics.Debug.WriteLine($"OK: {dir.Path}");
                } else
                {
                    System.Diagnostics.Debug.WriteLine($"  : {dir.Path}");
                }
            }
        }

        private IEnumerable<DirectoryDesc> DirectoryDescriptions()
        {
            var specialFolders = Enum.GetValues(typeof(Environment.SpecialFolder)).Cast<Environment.SpecialFolder>();
            return specialFolders.Select(s => new DirectoryDesc(s.ToString(), Environment.GetFolderPath(s))).Where(d => !string.IsNullOrEmpty(d.Path));
        }

        class DirectoryDesc
        {
            public DirectoryDesc(string key, string path)
            {
                Key = key;

                Path = path == null
                    ? ""
                    : string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), path.Split(System.IO.Path.DirectorySeparatorChar).Select(s => s.Length > 18 ? s.Substring(0, 5) + "..." + s.Substring(s.Length - 8, 8) : s));
            }

            public string Key { get; set; }
            public string Path { get; set; }
        }
    }

}
