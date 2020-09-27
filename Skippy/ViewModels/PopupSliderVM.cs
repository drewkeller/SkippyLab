using DynamicData.Binding;
using DynamicData.Kernel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;

namespace Skippy.ViewModels
{
    public class PopupSliderVM : ReactiveObject, IActivatableViewModel
    {
        [Reactive] public string Name { get; set; }

        [Reactive] public string Value { get; set; }

        public List<string> Values { get; set; }

        public ViewModelActivator Activator { get; set; }

        public PopupSliderVM()
        {
            Name = "Just Testing";
            Values = new List<string> { ".01", ".02", ".05", ".1", ".2", ".5", "1", "2", "5", "10", "20", "50", "100", "200", "500", "1000" };
            Value = ".1";

            Activator = new ViewModelActivator();

            this.WhenActivated(disposables =>
            {
                this.HandleActivation();

                Disposable
                    .Create(() => this.HandleDeactivation())
                    .DisposeWith(disposables);

            });

        }

        private void HandleDeactivation()
        {
        }

        private void HandleActivation()
        {
        }
    }

}
