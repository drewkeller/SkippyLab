// modified from https://github.com/Jouna77/Xamarin.Forms.UniformGrid

using System;
using Xamarin.Forms;

namespace Scoopy.Controls
{

    /// <summary>
    /// Provides a binding to <see cref="Switch.IsToggled"/> which isn't provided by the original control.
    /// </summary>
    public class SwitchEx : Switch
    {
        public new bool IsToggled { get; set; }
        public new static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
            nameof(IsToggled), typeof(bool), typeof(SwitchEx), propertyChanged: IsToggledPropertyChanged);
        private static void IsToggledPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchEx)bindable;
            if (bool.TryParse(newValue.ToString(), out var result))
            {
                control.IsToggled = result;
            }
        }
    }

}
namespace UniformGrid
{

    public class UniformGrid : Grid
    {

        public UniformGrid()
        {

        }

        #region Columns

        /// <summary>
        ///  Gets the number of columns that are in the grid.
        /// </summary>
        public int Columns { get; set; }
        public static readonly BindableProperty ColumnsProperty = BindableProperty.Create(
            propertyName: nameof(Columns), returnType: typeof(string), declaringType: typeof(UniformGrid),
            defaultValue:"", defaultBindingMode: BindingMode.TwoWay, propertyChanged: ColumnsPropertyChanged);
        private static void ColumnsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (UniformGrid)bindable;
            if (int.TryParse(newValue.ToString(), out var result))
            {
                control.Columns = result;
                control._explicitColumns = true;
                control.RefreshLayout();
            }
        }
        private bool _explicitColumns;

        #endregion

        #region Rows

        /// <summary>
        /// Gets or sets the number of rows that are in the grid.
        /// </summary>
        public int Rows { get; set; }
        public static readonly BindableProperty RowsProperty = BindableProperty.Create(
            propertyName: nameof(Rows), returnType: typeof(string), declaringType: typeof(UniformGrid),
            defaultValue:"", defaultBindingMode: BindingMode.TwoWay, propertyChanged: RowsPropertyChanged);
        private static void RowsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (UniformGrid)bindable;
            if (int.TryParse(newValue.ToString(), out var result))
            {
                control.Rows = result;
                control._explicitRows = true;
                control.RefreshLayout();
            }
        }
        private bool _explicitRows;

        #endregion

        #region Layout helpers for children

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);
        }

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);
            RefreshLayout();
        }

        protected override void OnChildRemoved(Element child)
        {
            base.OnChildRemoved(child);
            RefreshLayout();
        }

        #endregion

        private void RefreshLayout()
        {
            int row = 0;
            int column = 0;
            int numColumns = this.Children.Count;

            // Calculate number of columns based on explicit value of Rows or Columns
            // or make a "square" based on the number of children.
            if (_explicitRows && Rows > 0)
            {
                numColumns = (int)Math.Floor(this.Children.Count / (double)Rows);
            } 
            else if (_explicitColumns && Columns > 0)
            {
                numColumns = this.Columns;
            }
            else if (this.Children.Count >= 3)
            {
                numColumns = (int)Math.Ceiling(Math.Sqrt(this.Children.Count));
            }

            // Set row and column for each child
            for (int i = 0; i < this.Children.Count; i++)
            {
                Grid.SetColumn(this.Children[i], column);
                Grid.SetRow(this.Children[i], row);
                column++;
                if (column >= numColumns && (i + 1) < this.Children.Count)
                {
                    row++;
                    column = 0;
                }
            }

            // Update layout and properties
            InvalidateLayout();
            this.Rows = row + 1;
            this.Columns = numColumns;
        }
    }
}