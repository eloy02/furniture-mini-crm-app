using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FurnitureMiniCrm.App.Core.ViewModels;
using ReactiveUI;

namespace FurnitureMiniCrm.App.Avalonia.Views
{
    public class ProductsMainView : ReactiveUserControl<ProductsMainViewModel>
    {
        public ProductsMainView()
        {
            this.InitializeComponent();

            this.WhenActivated(disposables => { });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}