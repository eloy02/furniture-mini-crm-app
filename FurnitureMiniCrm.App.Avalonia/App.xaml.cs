using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaAppTemplate.Namespace;
using FurnitureMiniCrm.App.Avalonia.Views;
using FurnitureMiniCrm.App.Core.ViewModels;
using FurnitureMiniCrm.Services;
using ReactiveUI;
using Splat;

namespace FurnitureMiniCrm.App.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                    ViewModel = new MainWindowViewModel(),
                };
            }

            Locator.CurrentMutable.RegisterLazySingleton<IClientsService>(() => new ClientsService());
            Locator.CurrentMutable.RegisterLazySingleton<IProductsService>(() => new ProductsService());
            Locator.CurrentMutable.RegisterLazySingleton<IOrdersService>(() => new OrdersService());

            Locator.CurrentMutable.Register<IViewFor<ClientsMainViewModel>>(() => new ClientsMainView());
            Locator.CurrentMutable.Register<IViewFor<OrdersMainViewModel>>(() => new OrdersMainView());
            Locator.CurrentMutable.Register<IViewFor<ProductsMainViewModel>>(() => new ProductsMainView());
            Locator.CurrentMutable.Register<IViewFor<OrderFormViewModel>>(() => new OrderFormView());
            Locator.CurrentMutable.Register<IViewFor<ProductFormViewModel>>(() => new ProductFormView());
            Locator.CurrentMutable.Register<IViewFor<ClientFormViewModel>>(() => new ClientFormView());
            Locator.CurrentMutable.Register<IViewFor<CommonCataloguesViewModel>>(() => new CommonCataloguesView());

            base.OnFrameworkInitializationCompleted();
        }
    }
}