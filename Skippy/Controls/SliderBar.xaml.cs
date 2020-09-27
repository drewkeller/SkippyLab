using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Skippy.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SliderBar : ContentView
    {
        public SliderBar()
        {
            InitializeComponent();
        }
    }
}