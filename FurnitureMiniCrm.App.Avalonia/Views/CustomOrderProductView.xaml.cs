using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FurnitureMiniCrm.App.Core.ViewModels;
using ReactiveUI;

namespace AvaloniaAppTemplate.Namespace
{
    public class CustomOrderProductView : ReactiveWindow<CustomOrderProductViewModel>
    {
        public CustomOrderProductView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                ViewModel.AddProduct
                    .Where(x => x == true)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ => Close())
                    .DisposeWith(disposables);

                ViewModel.Cancel
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ => Close())
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}