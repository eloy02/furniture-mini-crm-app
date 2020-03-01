using FurnitureMiniCrm.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FurnitureMiniCrm.App.Core.ViewModels
{
    public sealed class SelectClientViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly IClientsService _clientsService;

        public ViewModelActivator Activator { get; }

        [Reactive]
        public string Fio { get; set; }

        [Reactive]
        public string PhoneNumber { get; set; }

        [Reactive]
        public ObservableCollection<ClientModel> Clients { get; set; }

        [Reactive]
        public ClientModel SelectedClient { get; set; }

        public ReactiveCommand<Unit, Unit> SelectClient { get; }

        public Interaction<bool, Unit> Close { get; }

        public SelectClientViewModel()
        {
            Activator = new ViewModelActivator();

            _clientsService = Locator.Current.GetService<IClientsService>();

            Close = new Interaction<bool, Unit>();

            var canSelectClient = this.WhenAnyValue(x => x.SelectedClient)
                .Select(client => client != null);
            SelectClient = ReactiveCommand.CreateFromObservable(() => Close.Handle(true), canSelectClient);

            var loadClients = ReactiveCommand.CreateFromTask(async () => await _clientsService.GetClientsAsync());

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.Fio, x => x.PhoneNumber)
                    .Throttle(TimeSpan.FromSeconds(0.5))
                    .SelectMany(_ => loadClients.Execute())
                    .Select(clients => FilterClients(clients))
                    .Select(filteredClients => new ObservableCollection<ClientModel>(filteredClients))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(filteredClients => Clients = filteredClients)
                    .DisposeWith(disposables);

                loadClients
                    .Execute()
                    .Select(FilterClients)
                    .Select(clients => new ObservableCollection<ClientModel>(clients))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(clients => Clients = clients)
                    .DisposeWith(disposables);
            });
        }

        private IEnumerable<ClientModel> FilterClients(IEnumerable<ClientModel> clients)
        {
            if (!string.IsNullOrWhiteSpace(Fio))
                clients = clients.Where(c => c.Fio.ToLower().Contains(Fio.ToLower()));

            if (!string.IsNullOrWhiteSpace(PhoneNumber))
                clients = clients.Where(c => c.PhoneNumber.Contains(PhoneNumber));

            return clients;
        }
    }
}