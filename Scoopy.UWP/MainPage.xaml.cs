namespace Scoopy.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new Scoopy.App());
        }
    }
}
