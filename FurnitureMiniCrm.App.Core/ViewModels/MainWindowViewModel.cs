using ReactiveUI;
using System.Reactive;

namespace FurnitureMiniCrm.App.Core.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IScreen
    {
        public RoutingState Router { get; } = new RoutingState();

        public ReactiveCommand<Unit, IRoutableViewModel> OpenClients { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> OpenOrders { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> OpenProducts { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> CreateOrder { get; }

        public MainWindowViewModel()
        {
            OpenClients = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(new ClientsMainViewModel(this)));

            OpenOrders = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(new OrdersMainViewModel(this)));

            OpenProducts = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(new ProductsMainViewModel(this)));

            CreateOrder = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(new OrderFormViewModel(this)));
        }
    }
}