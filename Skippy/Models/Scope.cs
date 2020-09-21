using System.Collections.Generic;
using Xamarin.Forms;

namespace Skippy
{
    public class Scope
    {
        public string Hostname { get; set; }

        public int Port { get; set; }

        public bool IsConnected { get; set; }

        public List<ScopeChannel> Channels { get; set; }

        public Image Screen { get; set; }

    }

}
