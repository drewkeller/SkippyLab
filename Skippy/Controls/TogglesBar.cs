/// <summary>
/// Based on the TogglesBar in the Xamlly.Controls project.
/// This adds property changed notification for <see cref="TogglesBar.SelectedItems"/> 
/// so two-way binding works properly.
/// </summary>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Skippy.Controls
{

    public class TogglesBarSelectionChangedEventArgs : EventArgs
    {
        public object SelectedItems { get; set; }
        public object SelectedIndices { get; set; }
    }

    public class TogglesBar : ContentView
    {
#if WPF
#else
        readonly ScrollView scrollContainer;
#endif
        readonly StackLayout stackContainer;
        public event EventHandler<TogglesBarSelectionChangedEventArgs> SelectedItemsChanged;

        public TogglesBar()
        {
#if WPF
#else
            scrollContainer = new ScrollView
            {
                Orientation = ScrollOrientation.Horizontal,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
            };
#endif

            stackContainer = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 0,
                Margin = 0,
                Padding = 0
            };
#if WPF
            Content = stackContainer;
#else
            scrollContainer.Content = stackContainer;
            Content = scrollContainer;
#endif
        }

        public int InitialIndex { get; set; } = -1;

#region Bindable Properties
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable<object>), typeof(TogglesBar),
          defaultBindingMode: BindingMode.TwoWay, propertyChanged: CustomPropertyChanging);

        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly BindableProperty SelectedItemsProperty = BindableProperty.Create(nameof(SelectedItems), typeof(object), typeof(TogglesBar),
  defaultBindingMode: BindingMode.TwoWay, propertyChanged: SelectedItemsPropertyChanged);
        public object SelectedItems
        {
            get { return GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly BindableProperty DisplayMemberPathProperty = BindableProperty.Create(nameof(DisplayMemberPath), typeof(string), typeof(TogglesBar),
            defaultBindingMode: BindingMode.OneTime,
            defaultValue: default(string),
            propertyChanged: CustomPropertyChanging);

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        public static readonly BindableProperty ItemsSpacingProperty =
        BindableProperty.Create(nameof(ItemsSpacing), typeof(double), typeof(TogglesBar), 0d);

        public double ItemsSpacing
        {
            get { return (double)GetValue(ItemsSpacingProperty); }
            set { SetValue(ItemsSpacingProperty, value); }
        }

        public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(TogglesBar), Button.FontFamilyProperty.DefaultValue);

        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static readonly BindableProperty IsMultiSelectProperty =
            BindableProperty.Create(nameof(IsMultiSelect), typeof(bool), typeof(TogglesBar), false);

        public bool IsMultiSelect
        {
            get { return (bool)GetValue(IsMultiSelectProperty); }
            set { SetValue(IsMultiSelectProperty, value); }
        }

        public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(TogglesBar),
            defaultValue: Color.Black, propertyChanged: CustomPropertyChanging);

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly BindableProperty UnselectedColorProperty = BindableProperty.Create(nameof(UnselectedColor), typeof(Color), typeof(TogglesBar),
            defaultValue: Color.Gray, propertyChanged: CustomPropertyChanging);


        public Color UnselectedColor
        {
            get { return (Color)GetValue(UnselectedColorProperty); }
            set { SetValue(UnselectedColorProperty, value); }
        }

        public static readonly BindableProperty InitialValuePathProperty =
           BindableProperty.Create(nameof(InitialValuePath), typeof(string), typeof(TogglesBar), null,
               propertyChanged: CustomPropertyChanging);

        public string InitialValuePath
        {
            get { return (string)GetValue(InitialValuePathProperty); }
            set { SetValue(InitialValuePathProperty, value); }
        }

        public static readonly BindableProperty InitialValueProperty =
            BindableProperty.Create(nameof(InitialValue), typeof(object), typeof(TogglesBar), null,
                propertyChanged: CustomPropertyChanging);

        public object InitialValue
        {
            get { return GetValue(InitialValueProperty); }
            set { SetValue(InitialValueProperty, value); }
        }
#endregion

        private static void CustomPropertyChanging(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != null)
                ((TogglesBar)bindable).Render();
        }

        private static void SelectedItemsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue == null) return;

            var control = bindable as TogglesBar;
            control.RenderSelectedItems();
        }

        /// <summary>
        /// Update which button(s) are selected (don't need to reacreate all of them)
        /// </summary>
        private void RenderSelectedItems()
        {
            if (ItemsSource == null || ItemsSource.Count() == 0)
                return;

            try
            {
                // determine which items should appear selected
                var textsToSelect = new List<string>();
                foreach (var item in ItemsSource)
                {
                    var displayText = DisplayMemberPath == null ? item.ToString() : item.GetType().GetProperty(DisplayMemberPath).GetValue(item, null).ToString();
                    if (ShouldSelectItem(item, displayText))
                    {
                        textsToSelect.Add(displayText);
                    }
                }

                // get all of the toggle buttons and apply IsSelected based on matching their Text
                var allToggleButtons = stackContainer.Children.Where(x => x is Xamlly.XamllyControls.ToggleButton);
                allToggleButtons?.ForEach(x =>
                {
                    var btn = x as Xamlly.XamllyControls.ToggleButton;
                    btn.IsSelected = textsToSelect.Contains(btn.Text);
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ShouldSelectItem(object item, string displayText)
        {
            if (!_initialRenderCompleted)
            {
                _initialRenderCompleted = true;
                if (!string.IsNullOrEmpty(InitialValuePath))
                {
                    var value = item.GetType().GetProperty(InitialValuePath).GetValue(item, null);
                    return value.ToString().Equals(InitialValue);
                }
                else if (InitialIndex >= 0)
                {
                    return ItemsSource.IndexOf(item) == InitialIndex;
                }
                else if (InitialValue != null)
                {
                    return item == InitialValue;
                }
            }

            if (IsMultiSelect)
                return (SelectedItems as IEnumerable<object>).Contains(displayText);
            else
                return SelectedItems?.ToString() == displayText;
        }

        private void Render()
        {
            try
            {
                if (ItemsSource == null || ItemsSource.Count() == 0)
                    return;

                object selectedItems = new ObservableCollection<object>();

                stackContainer.Children.Clear();

                foreach (var item in ItemsSource)
                {
                    var displayText = item.ToString();
                    if (DisplayMemberPath != null) {
                        var property = item.GetType().GetProperty(DisplayMemberPath);
                        if (property == null)
                        {
                            throw new InvalidOperationException($"The property selected by DisplayMemberPath ({DisplayMemberPath}) did not resolve.");
                        }
                        displayText = property.GetValue(item, null).ToString();
                    }
                    var btn = new Xamlly.XamllyControls.ToggleButton
                    {
                        Text = displayText,
                        FontFamily = FontFamily,
                        BackgroundColor = BackgroundColor,
                        SelectedColor = SelectedColor,
                        UnselectedColor = UnselectedColor,
                    };

                    btn.IsSelected = ShouldSelectItem(item, displayText);

                    btn.SelectionChanged += (s, e) =>
                    {
                        if (IsMultiSelect)
                        {
                            if (btn.IsSelected)
                                (selectedItems as ObservableCollection<object>).Add(item);
                            else
                                (selectedItems as ObservableCollection<object>).Remove(item);
                        }
                        else
                        {
                            var allToggleButtons = stackContainer.Children.Where(x => x is Xamlly.XamllyControls.ToggleButton);
                            allToggleButtons?.ForEach(x => ((Xamlly.XamllyControls.ToggleButton)x).IsSelected = false);
                            btn.IsSelected = true;
                            SelectedItems = item;
                        }

                        if (!IsMultiSelect && btn.IsSelected)
                        {
                            SelectedItemsChanged?.Invoke(this, new TogglesBarSelectionChangedEventArgs
                            {
                                SelectedItems = selectedItems,
                                SelectedIndices = ItemsSource.IndexOf(item)
                            });
                        }
                        else if (IsMultiSelect)
                        {
                            SelectedItemsChanged?.Invoke(this, new TogglesBarSelectionChangedEventArgs
                            {
                                SelectedItems = selectedItems,
                                SelectedIndices = (selectedItems as ObservableCollection<object>).Select(x => ItemsSource.IndexOf(x))
                            });
                        }
                    };
                    stackContainer.Spacing = ItemsSpacing;
                    stackContainer.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool _initialRenderCompleted;
    }
}
