﻿using ReactiveUI;
using ReactiveUI.XamForms;
using Skippy.Converters;
using Skippy.Extensions;
using Skippy.Protocols;
using Skippy.ViewModels;
using System;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Skippy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimebaseView : AccordionItemView, IViewFor<TimebaseVM>
    {
        public TimebaseVM ViewModel { get; set; }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = value as TimebaseVM; }

        public TimebaseView()
        {
            InitializeComponent();
            ViewModel = new TimebaseVM();

            var protocol = ViewModel.Protocol;

            // initialize some stuff
            Mode.ItemsSource = StringOptions.GetStringValues(protocol.Mode.Options);

            this.WhenActivated(disposable =>
            {
                this.BindToProperty(ViewModel,
                    vm => vm.Offset.Value, 
                    vm => vm.Offset.GetSucceeded,
                    v => v.Offset.Text, 
                    v => v.Offset.IsEnabled, 
                    disposable);
                //this.Bind(ViewModel,
                //    x => x.Offset.Value,
                //    x => x.Offset.Text)
                //    .DisposeWith(disposable);
                //this.Bind(ViewModel,
                //    x => x.Offset.GetSucceeded,
                //    x => x.Offset.IsEnabled)
                //    .DisposeWith(disposable);

                this.Bind(ViewModel,
                    x => x.Scale.Value,
                    x => x.uiScale.Text)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                    x => x.Scale.GetSucceeded,
                    x => x.uiScale.IsEnabled)
                    .DisposeWith(disposable);

                this.BindToProperty(ViewModel,
                    vm => vm.Mode.Value, 
                    vm => vm.Mode.GetSucceeded,
                    v => v.Mode.SelectedItems, 
                    v => v.Mode.IsEnabled,
                    vmToViewConverterOverride: new StringOptionsToStringConverter(),
                    disposable);
                //this.Bind(ViewModel,
                //    x => x.Mode.Value,
                //    x => x.Mode.SelectedItems,
                //    vmToViewConverterOverride: new StringOptionsToStringConverter())
                //    .DisposeWith(disposable);
                //this.Bind(ViewModel,
                //    x => x.Mode.GetSucceeded,
                //    x => x.Mode.IsEnabled)
                //    .DisposeWith(disposable);

                ViewModel.GetAll.Execute(null);
                WireEvents(disposable);
            });

        }

        private void WireEvents(CompositeDisposable disposable)
        {
            GetAllButton.Events().Clicked
                .Select(args => Unit.Default)
                .InvokeCommand(ViewModel.GetAll)
                .DisposeWith(disposable);
            SetAllButton.Events().Clicked
                .Select(args => Unit.Default)
                .InvokeCommand(ViewModel.SetAll)
                .DisposeWith(disposable);
        }
    }

}