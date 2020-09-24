using Skippy.Interfaces;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Skippy.Droid
{
    public class ScreenshotStorage : IScreenService
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
