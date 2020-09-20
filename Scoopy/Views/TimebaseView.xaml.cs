using ReactiveUI;
using ReactiveUI.XamForms;
using Scoopy.Converters;
using Scoopy.Protocols;
using Scoopy.ViewModels;
using System;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scoopy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimebaseView : ReactiveContentView<TimebaseVM>
    {
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
                    vmValToViewConverterOverride: new StringOptionsToStringConverter(),
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

    public static class ViewExtensions
    {

        /// <summary>
        /// IReactiveBinding<TView, TViewModel, (object? view, bool isViewModel)> 
        /// Bind<TViewModel, TView, TVMProp, TVProp>(
        ///   this TView                            view, 
        ///   TViewModel?                           viewModel, 
        ///   Expression<Func<TViewModel, TVMProp>> vmProperty, 
        ///   Expression<Func<TView, TVProp>>       viewProperty, 
        ///   object?                               conversionHint = null, 
        ///   IBindingTypeConverter?                vmToViewConverterOverride = null, 
        ///   IBindingTypeConverter?                viewToVMConverterOverride = null)
        /// where TViewModel : class
        /// where TView : class, IViewFor;
        /// </summary>
        //public static IReactiveBinding<TView, TViewModel, (object view, bool isViewModel)>
        public static void
            BindToProperty<TViewModel, TView, TVMValue, TVMCanExecute, TVProp, TVEnabledProp>(
                this TView view,
                TViewModel ViewModel, 
                Expression<Func<TViewModel, TVMValue>> vmCommand,
                Expression<Func<TViewModel, TVMCanExecute>> vmCanExecute,
                Expression<Func<TView, TVProp>> vProperty,
                Expression<Func<TView, TVEnabledProp>> vEnabled,
                CompositeDisposable disposables)
            where TViewModel : class
            where TView : class, IViewFor
        {
            //var command = ((MemberExpression)vmCommand.Body) as IScopeCommand<TValue>; // 
            view.Bind(ViewModel, vmCommand, vProperty).DisposeWith(disposables);
            view.Bind(ViewModel, vmCanExecute, vEnabled).DisposeWith(disposables);
        }

        public static void
            BindToProperty<TViewModel, TView, TVMValue, TVMCanExecute, TVProp, TVEnabledProp>(
                this TView view,
                TViewModel ViewModel, 
                Expression<Func<TViewModel, TVMValue>> vmCommand,
                Expression<Func<TViewModel, TVMCanExecute>> vmCanExecute,
                Expression<Func<TView, TVProp>> vProperty,
                Expression<Func<TView, TVEnabledProp>> vEnabled,
                IBindingTypeConverter vmValToViewConverterOverride,
                CompositeDisposable disposables)
            where TViewModel : class
            where TView : class, IViewFor
        {
            view.Bind(ViewModel, vmCommand, vProperty, vmToViewConverterOverride: vmValToViewConverterOverride)
                .DisposeWith(disposables);
            view.Bind(ViewModel, vmCanExecute, vEnabled).DisposeWith(disposables);
        }

    }

}