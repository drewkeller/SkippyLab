using Skippy.Interfaces;
using System;
using Xamarin.Essentials;

namespace Skippy.Interfaces
{
    public class ScreenService : IScreenService
    {

        public string ScreenshotFolder
        {
            get {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                //return Path.GetFullPath(Path.Combine(FileSystem.AppDataDirectory, "..", "AppData"));
            }
        }

    }
}
