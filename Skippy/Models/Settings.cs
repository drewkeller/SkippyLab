using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;

namespace Skippy.Models
{
    public class Settings : ReactiveObject
    {

        [Reactive] public string HostName { get; set; } = "192.168.1.104";

        [Reactive] public int Port { get; set; } = 5555;

        [Reactive]
        public string ScreenshotFolder { get; set; } =
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

    }
}
