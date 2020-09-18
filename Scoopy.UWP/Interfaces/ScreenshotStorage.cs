using System.IO;
using Xamarin.Essentials;

namespace Scoopy.UWP
{
    public class ScreenshotStorage : IScreenshotStorage
    {

        public string ScreenshotFolder { 
            get {
                return FileSystem.AppDataDirectory;
                //return Path.GetFullPath(Path.Combine(FileSystem.AppDataDirectory, "..", "AppData"));
            } 
        }

    }
}
