using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FurnitureMiniCrm.App.Core.ViewModels;

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
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}