using System;
using System.Reactive;
using System.Reactive.Disposables;
using ReactiveUI;

namespace FurnitureMiniCrm.App.Core.ViewModels
{
    public class MainWindowViewModel : ReactiveObject, IScreen, IActivatableViewModel
    {
        public RoutingState Router { get; } = new RoutingState();

        public ViewModelActivator Activator { get; }

        public ReactiveCommand<Unit, IRoutableViewModel> OpenClients { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> OpenOrders { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> OpenProducts { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> CreateOrder { get; }

        public MainWindowViewModel()
        {
            Activator = new ViewModelActivator();

            OpenClients = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(new ClientsMainViewModel(this)));

            OpenOrders = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(new OrdersMainViewModel(this)));

            OpenProducts = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(new ProductsMainViewModel(this)));

            CreateOrder = ReactiveCommand.CreateFromObservable(() =>
                Router.Navigate.Execute(new OrderFormViewModel(this)));

            this.WhenActivated(disposables =>
            {
                OpenOrders
                    .Execute()
                    .Subscribe()
                    .DisposeWith(disposables);
            });
        }
    }
}