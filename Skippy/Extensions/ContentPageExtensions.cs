using ReactiveUI.XamForms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Skippy.Extensions
{
    public static class ContentPageExtensions
    {

        /// <summary>
        /// Adds progress indicator, based on <see cref="IsBusy"/> on a View Model.
        /// At the end of the ContentPage, add the statement: this.AddProgressDisplay();
        /// See https://stackoverflow.com/questions/24885525/xamarin-forms-how-to-overlay-an-activityindicator-in-the-middle-of-a-stacklayo
        /// </summary>
        /// <param name="page"></param>
        public static void AddProgressDisplay<T>(this ReactiveContentPage<T> page, bool initialState = false) where T : class
        {
            //var content = page.Content;

            //var grid = new Grid();
            //grid.Children.Add(content);
            //var gridProgress = new Grid { BackgroundColor = Color.FromHex("#64FFE0B2"), Padding = new Thickness(50) };
            //gridProgress.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            //gridProgress.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            //gridProgress.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            //gridProgress.SetBinding(VisualElement.IsVisibleProperty, "IsBusy");
            //var activity = new ActivityIndicator
            //{
            //    IsEnabled = true,
            //    IsVisible = initialState,
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    IsRunning = initialState
            //};
            //gridProgress.Children.Add(activity, 0, 1);
            //grid.Children.Add(gridProgress);
            //page.Content = grid;
        }
    }

}
