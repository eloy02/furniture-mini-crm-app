using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FurnitureMiniCrm.App.Core.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IScreen, IActivatableViewModel
    {
        public RoutingState Router { get; } = new RoutingState();

        public ViewModelActivator Activator { get; }

        [Reactive]
        public bool CanCreateOrder { get; set; }

        [Reactive]
        public bool CanOpenOrdersList { get; set; }

        [Reactive]
        public bool CanOpenClientsList { get; set; }

        [Reactive]
        public bool CanOpenProductsList { get; set; }

        public ReactiveCommand<Unit, IRoutableViewModel> OpenClients { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> OpenOrders { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> OpenProducts { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NewOrder { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NewProduct { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NewClient { get; }

        public MainWindowViewModel()
        {
            Activator = new ViewModelActivator();

            OpenClients = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(new ClientsMainViewModel(this)));

            OpenOrders = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(new OrdersMainViewModel(this)));

            OpenProducts = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(new ProductsMainViewModel(this)));

            NewOrder = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(new OrderFormViewModel(this)));

            NewProduct = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(new ProductFormViewModel(this)));

            NewClient = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(new ClientFormViewModel(this)));

            //this.WhenActivated(disposables =>
            //{
            //});
        }
    }
}