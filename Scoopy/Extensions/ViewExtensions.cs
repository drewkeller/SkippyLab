using ReactiveUI;
using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;

namespace Scoopy.Extensions
{
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

        /// <summary>
        /// Binds the value of a <see cref="IScopeCommand"></see> in the ViewModel to the value
        /// of a UI element in the View, automatically disabling it when the command is not
        /// available.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TVMValue"></typeparam>
        /// <typeparam name="TVMCanExecute"></typeparam>
        /// <typeparam name="TVProp"></typeparam>
        /// <typeparam name="TVEnabledProp"></typeparam>
        /// <param name="view">The view from which to do the binding, usually called with 'this'</param>
        /// <param name="viewModel">The ViewModel which contains the command that is being bound</param>
        /// <param name="vmCommandValue"><seealso cref="ScopeCommand{T}.Value"/></param>
        /// <param name="vmCommandCanExecute"><seealso cref="ScopeCommand{T}.GetSucceeded"/></param>
        /// <param name="vProperty">The UI property that is being bound, e.g. MyEntry.Text</param>
        /// <param name="vEnabled">The UI property that determines enabled state, e.g. MyEntry.IsEnabled</param>
        /// <param name="vmToViewConverterOverride"></param>
        /// <param name="disposables">An <seealso cref="IBindingTypeConverter"/> to use when converting the value from
        /// the ViewModel to the View</param>
        public static void
            BindToProperty<TViewModel, TView, TVMValue, TVMCanExecute, TVProp, TVEnabledProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TVMValue>> vmCommand,
                Expression<Func<TViewModel, TVMCanExecute>> vmCommandCanExecute,
                Expression<Func<TView, TVProp>> vProperty,
                Expression<Func<TView, TVEnabledProp>> vEnabled,
                CompositeDisposable disposables)
            where TViewModel : class
            where TView : class, IViewFor
        {
            //var command = ((MemberExpression)vmCommand.Body) as IScopeCommand<TValue>; // 
            view.Bind(viewModel, vmCommand, vProperty).DisposeWith(disposables);
            view.Bind(viewModel, vmCommandCanExecute, vEnabled).DisposeWith(disposables);
        }

        /// <summary>
        /// Binds the value of a <see cref="IScopeCommand"></see> in the ViewModel to the value
        /// of a UI element in the View, automatically disabling it when the command is not
        /// available.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TVMValue"></typeparam>
        /// <typeparam name="TVMCanExecute"></typeparam>
        /// <typeparam name="TVProp"></typeparam>
        /// <typeparam name="TVEnabledProp"></typeparam>
        /// <param name="view">The view from which to do the binding, usually called with 'this'</param>
        /// <param name="viewModel">The ViewModel which contains the command that is being bound</param>
        /// <param name="vmCommandValue"><seealso cref="ScopeCommand{T}.Value"/></param>
        /// <param name="vmCommandCanExecute"><seealso cref="ScopeCommand{T}.GetSucceeded"/></param>
        /// <param name="vProperty">The UI property that is being bound, e.g. MyEntry.Text</param>
        /// <param name="vEnabled">The UI property that determines enabled state, e.g. MyEntry.IsEnabled</param>
        /// <param name="vmToViewConverterOverride"></param>
        /// <param name="disposables">An <seealso cref="IBindingTypeConverter"/> to use when converting the value from
        /// the ViewModel to the View</param>
        public static void
            BindToProperty<TViewModel, TView, TVMValue, TVMCanExecute, TVProp, TVEnabledProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TVMValue>> vmCommandValue,
                Expression<Func<TViewModel, TVMCanExecute>> vmCommandCanExecute,
                Expression<Func<TView, TVProp>> vProperty,
                Expression<Func<TView, TVEnabledProp>> vEnabled,
                IBindingTypeConverter vmToViewConverterOverride,
                CompositeDisposable disposables)
            where TViewModel : class
            where TView : class, IViewFor
        {
            view.Bind(viewModel, vmCommandValue, vProperty, vmToViewConverterOverride: vmToViewConverterOverride)
                .DisposeWith(disposables);
            view.Bind(viewModel, vmCommandCanExecute, vEnabled).DisposeWith(disposables);
        }

    }
}
