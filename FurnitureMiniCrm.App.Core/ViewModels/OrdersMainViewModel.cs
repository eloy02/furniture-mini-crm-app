using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using FurnitureMiniCrm.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace FurnitureMiniCrm.App.Core.ViewModels
{
    public sealed class OrdersMainViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
    {
        private readonly IOrdersService _ordersService;

        private readonly SourceCache<OrderModel, int> _ordersSource;
        private readonly SourceCache<OrderProductModel, int> _orderItemsSource;
        private readonly SourceCache<CustomOrderProductModel, int> _customOrderItemsSource;
        private readonly ReadOnlyObservableCollection<OrderModel> _orders;
        private readonly ReadOnlyObservableCollection<OrderProductModel> _orderItems;
        private readonly ReadOnlyObservableCollection<CustomOrderProductModel> _customOrderItems;

        public string UrlPathSegment { get; } = "/orders";
        public IScreen HostScreen { get; }
        public ViewModelActivator Activator { get; }

        public ReactiveCommand<Unit, IRoutableViewModel> CreateNewOrder { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> EditOrder { get; }

        public ReadOnlyObservableCollection<OrderModel> Orders => _orders;
        public ReadOnlyObservableCollection<OrderProductModel> OrderItems => _orderItems;
        public ReadOnlyObservableCollection<CustomOrderProductModel> CustomOrderItems => _customOrderItems;

        [Reactive]
        public OrderModel SelectedOrder { get; set; }

        public OrdersMainViewModel(IScreen hostScreen = null)
        {
            HostScreen = hostScreen;

            Activator = new ViewModelActivator();

            if (HostScreen is null)
            {
                HostScreen = Locator.Current.GetService<IScreen>();
            }

            _ordersService = Locator.Current.GetService<IOrdersService>();

            _ordersSource = new SourceCache<OrderModel, int>(x => x.Id);
            _orderItemsSource = new SourceCache<OrderProductModel, int>(x => x.Id);
            _customOrderItemsSource = new SourceCache<CustomOrderProductModel, int>(x => x.Id);

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

            _customOrderItemsSource
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _customOrderItems)
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
                    .Do(_ => _orderItemsSource.Clear())
                    .Where(order => order != null)
                    .Where(order => order.Products != null && order.Products.Count > 0)
                    .Select(order => order.Products)
                    .Subscribe(orderItems => _orderItemsSource.AddOrUpdate(orderItems))
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.SelectedOrder)
                    .Do(_ => _customOrderItemsSource.Clear())
                    .Where(order => order != null)
                    .Where(order => order.CustomProducts != null && order.CustomProducts.Count > 0)
                    .Select(order => order.CustomProducts)
                    .Subscribe(items => _customOrderItemsSource.AddOrUpdate(items))
                    .DisposeWith(disposables);

                Disposable.Create(() =>
                {
                    _ordersSource?.Clear();
                    //_ordersSource?.Dispose();

                    _orderItemsSource?.Clear();
                    //_orderItemsSource?.Dispose();

                    _customOrderItemsSource?.Clear();
                    //_customOrderItemsSource?.Dispose();

                    SelectedOrder = null;
                }).DisposeWith(disposables);
            });
        }
    }
}