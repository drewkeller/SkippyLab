namespace Skippy.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            var brush = this.Background as Windows.UI.Xaml.Media.SolidColorBrush;
            var color = brush.Color;
            AppLocator.BackgroundColor = Xamarin.Forms.Color.FromRgb(color.R, color.G, color.B);
            LoadApplication(new Skippy.App());
        }
    }
}
