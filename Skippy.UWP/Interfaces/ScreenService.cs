using Skippy.Interfaces;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Skippy.UWP
{
    public class ScreenService : IScreenService
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
