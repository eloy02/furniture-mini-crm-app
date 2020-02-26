using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FurnitureMiniCrm.App.Core.ViewModels;

namespace FurnitureMiniCrm.App.Avalonia.Views
{
    public class OrdersMainView : ReactiveUserControl<OrdersMainViewModel>
    {
        public OrdersMainView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}