using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using FurnitureMiniCrm.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace FurnitureMiniCrm.App.Core.ViewModels
{
    public class OrderItemViewModel : ReactiveObject
    {
        public OrderItemViewModel(int id, ProductModel product)
        {
            Id = id;
            Product = product;
            Size = product.Size;

            this.WhenAnyValue(x => x.Count)
                .Select(count => count * Product.SellPrice)
                .Subscribe(price => TotalPrice = price);
        }

        public OrderItemViewModel(OrderProductModel orderProduct)
        {
            Id = orderProduct.Id;
            Product = orderProduct.Product;
            Count = orderProduct.Count;
            Size = orderProduct.Size;

            this.WhenAnyValue(x => x.Count)
                .Select(count => count * Product.SellPrice)
                .Subscribe(price => TotalPrice = price);
        }

        public int Id { get; set; }

        public ProductModel Product { get; set; }

        [Reactive]
        public int Count { get; set; }

        [Reactive]
        public double TotalPrice { get; set; }

        [Reactive]
        public string Size { get; set; }
    }

    public class CustomOrderItemViewModel : ReactiveObject
    {
        public CustomOrderItemViewModel(CustomOrderProductModel customProduct)
        {
            Id = customProduct.Id;
            Product = customProduct;
            Count = customProduct.Count;
            Size = customProduct.Size;

            this.WhenAnyValue(x => x.Count)
                .Select(c => c * Product.SellPrice)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(total => TotalPrice = total);
        }

        public int Id { get; set; }
        public CustomOrderProductModel Product { get; set; }

        [Reactive]
        public int Count { get; set; }

        [Reactive]
        public double TotalPrice { get; private set; }

        [Reactive]
        public string Size { get; set; }
    }

    public class OrderFormViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
    {
        private readonly IOrdersService _ordersService;

        private readonly SourceCache<OrderItemViewModel, int> _orderItemsSource;
        private readonly SourceCache<CustomOrderItemViewModel, int> _customOrderItemsSource;

        [Reactive]
        private OrderModel Order { get; set; }

        public string UrlPathSegment { get; } = "/orders/form";
        public IScreen HostScreen { get; }
        public ViewModelActivator Activator { get; }

        public ReactiveCommand<Unit, Unit> SelectClientCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveProductCommand { get; }
        public ReactiveCommand<Unit, Unit> SelectProductCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveOrder { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public ReactiveCommand<Unit, Unit> AddCustomProductCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveCustomProductCommand { get; }

        public Interaction<SelectClientViewModel, Unit> SelectClient { get; }
        public Interaction<SelectProductViewModel, Unit> SelectProduct { get; }
        public Interaction<CustomOrderProductViewModel, Unit> AddCustomProduct { get; }

        [Reactive]
        public string OrderNumber { get; set; }

        [Reactive]
        public DateTime? CreateDate { get; set; }

        [Reactive]
        public double OrderSumm { get; set; }

        [Reactive]
        public ClientModel SelectedClient { get; set; }

        [Reactive]
        public ObservableCollection<OrderStatusModel> OrderStatuses { get; set; }

        [Reactive]
        public OrderStatusModel SelectedOrderStatus { get; set; }

        private readonly ReadOnlyObservableCollection<OrderItemViewModel> _orderItems;
        public ReadOnlyObservableCollection<OrderItemViewModel> OrderItems => _orderItems;

        private readonly ReadOnlyObservableCollection<CustomOrderItemViewModel> _customOrderItems;
        public ReadOnlyObservableCollection<CustomOrderItemViewModel> CustomOrderItems => _customOrderItems;

        [Reactive]
        public OrderItemViewModel SelectedProduct { get; set; }

        [Reactive]
        public CustomOrderItemViewModel SelectedCustomProduct { get; set; }

        public OrderFormViewModel(IScreen hostScreen = null, ClientModel clientForOrder = null, OrderModel orderForEdit = null)
        {
            HostScreen = hostScreen;

            if (HostScreen is null)
            {
                HostScreen = Locator.Current.GetService<IScreen>();
            }

            _ordersService = Locator.Current.GetService<IOrdersService>();

            Activator = new ViewModelActivator();

            SelectClient = new Interaction<SelectClientViewModel, Unit>();
            SelectProduct = new Interaction<SelectProductViewModel, Unit>();
            AddCustomProduct = new Interaction<CustomOrderProductViewModel, Unit>();

            Cancel = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.NavigateBack.Execute());
            SelectClientCommand = ReactiveCommand.CreateFromTask(() => SelectClientAsync());
            SelectProductCommand = ReactiveCommand.CreateFromTask(() => SelectProductAsync());

            var canRemoveProduct = this.WhenAnyValue(x => x.SelectedProduct)
                .Select(x => x != null);

            RemoveProductCommand = ReactiveCommand.Create(() =>
                _orderItemsSource.Remove(SelectedProduct), canRemoveProduct);

            AddCustomProductCommand = ReactiveCommand.CreateFromObservable(() =>
            {
                var vm = Locator.Current.GetService<CustomOrderProductViewModel>();

                vm.OnOrderProductCreated = item =>
                {
                    item.Id = _customOrderItemsSource.Count;
                    _customOrderItemsSource.AddOrUpdate(new CustomOrderItemViewModel(item));
                };

                return AddCustomProduct.Handle(vm);
            });

            var canRemoveCustomProduct = this.WhenAnyValue(x => x.SelectedCustomProduct)
                    .Select(x => x != null);
            RemoveCustomProductCommand = ReactiveCommand.Create(() =>
                _customOrderItemsSource.Remove(SelectedCustomProduct), canExecute: canRemoveCustomProduct);

            var canSaveOrder = this.WhenAnyValue(
                x => x.SelectedOrderStatus,
                x => x.SelectedClient,
                (status, client) =>
                    status != null
                    && client != null);

            SaveOrder = ReactiveCommand.CreateFromTask(async () =>
            {
                Order.Client = SelectedClient;
                Order.Status = SelectedOrderStatus;
                Order.Sum = OrderSumm;
                Order.Products = _orderItemsSource.Items
                    .Select(o => new OrderProductModel()
                    {
                        Id = _orderItemsSource.Items.IndexOf(o),
                        Count = o.Count,
                        Product = o.Product,
                        TotalPrice = o.TotalPrice,
                        Size = o.Size
                    })
                    .ToList();
                Order.CustomProducts = _customOrderItemsSource.Items
                    .Select(o => new CustomOrderProductModel()
                    {
                        Id = o.Id,
                        Count = o.Count,
                        Name = o.Product.Name,
                        Price = o.Product.Price,
                        SellPrice = o.Product.SellPrice,
                        Size = o.Size,
                        TotalPrice = o.TotalPrice,
                        Unit = o.Product.Unit
                    }).ToList();

                await _ordersService.SetOrderAsync(Order);

                await HostScreen.Router.NavigateBack.Execute();
            }, canSaveOrder);

            _orderItemsSource = new SourceCache<OrderItemViewModel, int>(x => x.Id);

            _orderItemsSource
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _orderItems)
                .DisposeMany()
                .Subscribe();

            _customOrderItemsSource = new SourceCache<CustomOrderItemViewModel, int>(x => x.Id);

            _customOrderItemsSource
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _customOrderItems)
                .DisposeMany()
                .Subscribe();

            var loadOrderStatuses = ReactiveCommand.CreateFromTask(async () =>
                await _ordersService.GetOrderStatusesAsync());

            var getNewOrder = ReactiveCommand.CreateFromTask(async () => await _ordersService.CreateNewOrder());

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.Order)
                    .Where(order => order != null)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(order =>
                    {
                        OrderNumber = order.Id.ToString();
                        CreateDate = order.CreateDate;
                        SelectedClient = order.Client;

                        if (order.Products != null)
                        {
                            _orderItemsSource.AddOrUpdate(order.Products.Select(p => new OrderItemViewModel(p)));
                        }

                        if (order.CustomProducts != null)
                        {
                            _customOrderItemsSource.AddOrUpdate(
                                order.CustomProducts.Select(p => new CustomOrderItemViewModel(p)));
                        }
                    })
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.OrderStatuses)
                    .Where(statuses => statuses != null)
                    .Where(_ => Order.Status is null)
                    .Select(statuses => statuses.FirstOrDefault())
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(status => SelectedOrderStatus = status)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.OrderStatuses)
                    .Where(statuses => statuses != null)
                    .Where(_ => Order.Status != null)
                    .Select(statuses => statuses.SingleOrDefault(st => st.Id == Order.Status.Id))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(status => SelectedOrderStatus = status)
                    .DisposeWith(disposables);

                if (orderForEdit != null)
                {
                    Order = orderForEdit;
                }

                if (orderForEdit is null)
                {
                    getNewOrder
                        .Execute()
                        .Subscribe(order => Order = order)
                        .DisposeWith(disposables);
                }

                loadOrderStatuses
                    .Execute()
                    .Select(statuses => new ObservableCollection<OrderStatusModel>(statuses))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(statuses => OrderStatuses = statuses)
                    .DisposeWith(disposables);

                if (clientForOrder != null)
                {
                    SelectedClient = clientForOrder;
                }

                Observable.Timer(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.3))
                    .SelectMany(_ => CalculateTotalSummAsync())
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(totalSum => OrderSumm = totalSum)
                    .DisposeWith(disposables);

                Disposable.Create(() =>
                {
                    _orderItemsSource.Clear();
                }).DisposeWith(disposables);
            });
        }

        private async Task SelectProductAsync()
        {
            SelectProductViewModel selectProductVm = new SelectProductViewModel();

            await SelectProduct.Handle(selectProductVm);

            if (selectProductVm.SelectedProduct != null)
            {
                _orderItemsSource.AddOrUpdate(new OrderItemViewModel(_orderItemsSource.Count, selectProductVm.SelectedProduct));
            }
        }

        private async Task SelectClientAsync()
        {
            SelectClientViewModel selectClientVm = new SelectClientViewModel();

            await SelectClient.Handle(selectClientVm);

            if (selectClientVm.SelectedClient != null)
            {
                SelectedClient = selectClientVm.SelectedClient;
            }
        }

        private async Task<double> CalculateTotalSummAsync()
        {
            var productsSumm = Task.Run(() =>
                _orderItemsSource.Items
                    .Sum(x => x.TotalPrice));

            var customProductsSum = Task.Run(() =>
                _customOrderItemsSource.Items
                    .Sum(x => x.TotalPrice));

            await Task.WhenAll(productsSumm, customProductsSum);

            return productsSumm.Result + customProductsSum.Result;
        }
    }
}