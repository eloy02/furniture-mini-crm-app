using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using DynamicData;
using FurnitureMiniCrm.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace FurnitureMiniCrm.App.Core.ViewModels
{
    public class ClientsMainViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
    {
        private readonly IClientsService _clientsService;
        private readonly IOrdersService _ordersService;

        private readonly SourceCache<ClientModel, int> _clientsSource;
        private readonly SourceCache<OrderModel, int> _clientOrdersSource;

        private readonly ReadOnlyObservableCollection<ClientModel> _clients;
        private readonly ReadOnlyObservableCollection<OrderModel> _clientOrders;

        public ViewModelActivator Activator { get; }
        public string UrlPathSegment { get; } = "/clients";
        public IScreen HostScreen { get; }

        public ReactiveCommand<Unit, IRoutableViewModel> CreateClient { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> EditClient { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> NewOrderForClient { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> EditClientOrder { get; }

        public ReadOnlyObservableCollection<ClientModel> Clients => _clients;
        public ReadOnlyObservableCollection<OrderModel> ClientOrders => _clientOrders;

        [Reactive]
        public ClientModel SelectedClient { get; set; }

        [Reactive]
        public OrderModel SelectedClientOrder { get; set; }

        public ClientsMainViewModel(IScreen hostScreen = null)
        {
            HostScreen = hostScreen;

            if (HostScreen is null)
            {
                HostScreen = Locator.Current.GetService<IScreen>();
            }

            _clientsService = Locator.Current.GetService<IClientsService>();

            _ordersService = Locator.Current.GetService<IOrdersService>();

            Activator = new ViewModelActivator();

            _clientsSource = new SourceCache<ClientModel, int>(x => x.Id);

            _clientOrdersSource = new SourceCache<OrderModel, int>(x => x.Id);

            CreateClient = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new ClientFormViewModel(HostScreen)));

            var canEditClient = this.WhenAnyValue(x => x.SelectedClient)
                .Select(client => client != null);

            EditClient = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new ClientFormViewModel(HostScreen, SelectedClient)), canEditClient);

            NewOrderForClient = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new OrderFormViewModel(HostScreen, SelectedClient)), canEditClient);

            var canEditClientOrder = this.WhenAnyValue(x => x.SelectedClientOrder)
                    .Select(order => order != null);

            EditClientOrder = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new OrderFormViewModel(HostScreen, orderForEdit: SelectedClientOrder)), canEditClientOrder);

            var loadClients = ReactiveCommand.CreateFromTask(async () => await _clientsService.GetClientsAsync());

            _clientsSource
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _clients)
                .DisposeMany()
                .Subscribe();

            _clientOrdersSource
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _clientOrders)
                .DisposeMany()
                .Subscribe();

            this.WhenActivated(disposables =>
            {
                Disposable.Create(() =>
                {
                    _clientsSource?.Clear();
                    _clientOrdersSource?.Clear();
                }).DisposeWith(disposables);

                _clientsSource.PopulateFrom(loadClients.Execute());

                _clientsService.NewClients
                    .Where(client => client != null)
                    .Subscribe(client => _clientsSource.AddOrUpdate(client))
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.SelectedClient)
                    .Do(_ => _clientOrdersSource.Clear())
                    .Where(client => client != null)
                    .Subscribe(client =>
                        _clientOrdersSource.PopulateFrom(_ordersService.GetOrdersAsync(client).ToObservable()))
                    .DisposeWith(disposables);

                _ordersService.NewOrders
                    .Where(_ => SelectedClient != null)
                    .Where(order => order != null)
                    .Where(order => order.Client != null && order.Client.Id == SelectedClient.Id)
                    .Subscribe(order => _clientOrdersSource.AddOrUpdate(order))
                    .DisposeWith(disposables);

                Disposable.Create(() => _clientsSource.Clear())
                    .DisposeWith(disposables);
            });
        }
    }
}