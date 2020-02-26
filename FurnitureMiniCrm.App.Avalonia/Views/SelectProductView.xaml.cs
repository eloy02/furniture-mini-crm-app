using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FurnitureMiniCrm.App.Core.ViewModels;

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
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}