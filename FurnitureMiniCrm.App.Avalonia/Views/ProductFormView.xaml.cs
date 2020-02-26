using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FurnitureMiniCrm.App.Core.ViewModels;
using ReactiveUI;

namespace FurnitureMiniCrm.App.Avalonia.Views
{
    public class ProductFormView : ReactiveUserControl<ProductFormViewModel>
    {
        public ProductFormView()
        {
            InitializeComponent();

            this.WhenActivated(disposables => { });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}