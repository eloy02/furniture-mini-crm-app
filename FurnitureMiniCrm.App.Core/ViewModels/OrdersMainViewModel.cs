using DynamicData;
using FurnitureMiniCrm.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FurnitureMiniCrm.App.Core.ViewModels
{
    public sealed class OrdersMainViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
    {
        private readonly IOrdersService _ordersService;

        private readonly SourceCache<OrderModel, int> _ordersSource;
        private readonly SourceCache<OrderProductModel, int> _orderItemsSource;
        private readonly ReadOnlyObservableCollection<OrderModel> _orders;
        private readonly ReadOnlyObservableCollection<OrderProductModel> _orderItems;

        public string UrlPathSegment { get; } = "/orders";
        public IScreen HostScreen { get; }
        public ViewModelActivator Activator { get; }

        public ReactiveCommand<Unit, IRoutableViewModel> CreateNewOrder { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> EditOrder { get; }

        public ReadOnlyObservableCollection<OrderModel> Orders => _orders;
        public ReadOnlyObservableCollection<OrderProductModel> OrderItems => _orderItems;

        [Reactive]
        public OrderModel SelectedOrder { get; set; }

        public OrdersMainViewModel(IScreen hostScreen = null)
        {
            HostScreen = hostScreen;

            Activator = new ViewModelActivator();

            if (HostScreen is null)
                HostScreen = Locator.Current.GetService<IScreen>();

            _ordersService = Locator.Current.GetService<IOrdersService>();

            _ordersSource = new SourceCache<OrderModel, int>(x => x.Id);
            _orderItemsSource = new SourceCache<OrderProductModel, int>(x => x.Id);

            _ordersSource
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _orders)
                .DisposeMany()
                .Subscribe();

            _orderItemsSource
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _orderItems)
                .DisposeMany()
                .Subscribe();

            CreateNewOrder = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new OrderFormViewModel(HostScreen)));

            var canEditOrder = this.WhenAnyValue(x => x.SelectedOrder)
                .Select(x => x != null);

            EditOrder = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new OrderFormViewModel(HostScreen, orderForEdit: SelectedOrder)), canEditOrder);

            var loadOrders = ReactiveCommand.CreateFromTask(async () =>
                await _ordersService.GetOrdersAsync());

            this.WhenActivated(disposables =>
            {
                _ordersSource.PopulateFrom(loadOrders.Execute());

                this.WhenAnyValue(x => x.SelectedOrder)
                    .Where(order => order != null)
                    .Select(order => order.Products)
                    .Subscribe(orderItems => _orderItemsSource.AddOrUpdate(orderItems))
                    .DisposeWith(disposables);

                Disposable.Create(() =>
                {
                    _ordersSource.Clear();
                }).DisposeWith(disposables);
            });
        }
    }
}