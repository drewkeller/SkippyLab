using Acr.UserDialogs;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Skippy.Extensions;
using Skippy.Interfaces;
using Skippy.Models;
using Splat;
using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Skippy.ViewModels
{
    public class ScopeVM : ReactiveObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; }

        public ScopeVM()
        {
            Activator = new ViewModelActivator();

            this.WhenActivated(disposables =>
            {
                this.HandleActivation();

                Disposable
                    .Create(() => this.HandleDeactivation())
                    .DisposeWith(disposables);

            });
        }

        private void HandleActivation()
        {
        }

        private void HandleDeactivation()
        {
        }
    }

}
