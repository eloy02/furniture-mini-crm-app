using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FurnitureMiniCrm.App.Core.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FurnitureMiniCrm.App.Avalonia.Views
{
    public class SelectClientView : ReactiveWindow<SelectClientViewModel>
    {
        public SelectClientView()
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
                        this.Close();

                    interaction.SetOutput(new System.Reactive.Unit());
                })
                .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}