using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scoopy.Models
{
    public class Settings: ReactiveObject
    {

        [Reactive] public string HostName { get; set; } = "192.168.1.105";

        [Reactive] public int Port { get; set; } = 5555;

    }
}
