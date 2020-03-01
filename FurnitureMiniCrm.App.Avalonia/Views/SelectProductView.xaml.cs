using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FurnitureMiniCrm.App.Core.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FurnitureMiniCrm.App.Avalonia.Views
{
    public class SelectProductView : ReactiveWindow<SelectProductViewModel>
    {
        public SelectProductView()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.WhenActivated(disposables =>
            {
                this.ViewModel.Close.RegisterHandler(interaction =>
                {
                    if (interaction.Input == true)
                        Close();

                    interaction.SetOutput(new System.Reactive.Unit());
                }).DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}