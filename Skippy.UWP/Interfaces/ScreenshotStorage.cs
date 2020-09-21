using System.IO;
using Xamarin.Essentials;

namespace Skippy.UWP
{
    public class ScreenshotStorage : IScreenshotStorage
    {

        public string ScreenshotFolder
        {
            get {
                return FileSystem.AppDataDirectory;
                //return Path.GetFullPath(Path.Combine(FileSystem.AppDataDirectory, "..", "AppData"));
            }
        }

    }
}
