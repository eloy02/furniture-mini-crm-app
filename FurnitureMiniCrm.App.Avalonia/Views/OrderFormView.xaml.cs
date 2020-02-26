using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FurnitureMiniCrm.App.Core.ViewModels;
using ReactiveUI;
using System.Reactive;

namespace FurnitureMiniCrm.App.Avalonia.Views
{
    public class OrderFormView : ReactiveUserControl<OrderFormViewModel>
    {
        public OrderFormView()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.ViewModel
                    .SelectClient
                    .RegisterHandler(async interaction =>
                    {
                        var selectClientView = new SelectClientView()
                        {
                            ViewModel = interaction.Input
                        };

                        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                        {
                            await selectClientView.ShowDialog(desktop.MainWindow);
                        }

                        interaction.SetOutput(new Unit());
                    });

                this.ViewModel
                    .SelectProduct
                    .RegisterHandler(async interaction =>
                    {
                        var selectProductView = new SelectProductView()
                        {
                            ViewModel = interaction.Input
                        };

                        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                        {
                            await selectProductView.ShowDialog(desktop.MainWindow);
                        }

                        interaction.SetOutput(new Unit());
                    });
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}