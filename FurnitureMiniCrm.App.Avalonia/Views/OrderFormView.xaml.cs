using System.Reactive;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using AvaloniaAppTemplate.Namespace;
using FurnitureMiniCrm.App.Core.ViewModels;
using ReactiveUI;

namespace FurnitureMiniCrm.App.Avalonia.Views
{
    public class OrderFormView : ReactiveUserControl<OrderFormViewModel>
    {
        public OrderFormView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                ViewModel
                    .SelectClient
                    .RegisterHandler(async interaction =>
                    {
                        var selectClientView = new SelectClientView()
                        {
                            DataContext = interaction.Input,
                            ViewModel = interaction.Input
                        };

                        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                        {
                            await selectClientView.ShowDialog(desktop.MainWindow);
                        }

                        interaction.SetOutput(new Unit());
                    });

                ViewModel
                    .SelectProduct
                    .RegisterHandler(async interaction =>
                    {
                        var selectProductView = new SelectProductView()
                        {
                            DataContext = interaction.Input,
                            ViewModel = interaction.Input
                        };

                        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                        {
                            await selectProductView.ShowDialog(desktop.MainWindow);
                        }

                        interaction.SetOutput(new Unit());
                    });

                ViewModel
                    .AddCustomProduct
                    .RegisterHandler(async interaction =>
                    {
                        var view = new CustomOrderProductView()
                        {
                            DataContext = interaction.Input,
                            ViewModel = interaction.Input
                        };

                        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                        {
                            await view.ShowDialog(desktop.MainWindow);
                        }

                        interaction.SetOutput(new Unit());
                    }).DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}