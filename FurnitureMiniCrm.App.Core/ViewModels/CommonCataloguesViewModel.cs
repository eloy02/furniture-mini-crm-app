using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class CommonCataloguesViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
    {
        public ViewModelActivator Activator { get; }
        public string UrlPathSegment => "/catalogues";
        public IScreen HostScreen { get; }

        private readonly IProductsService _productsService;
        private readonly IOrdersService _ordersService;

        #region Order Statuses

        private readonly SourceCache<OrderStatusModel, int> _orderStatusesSource;
        private readonly ReadOnlyObservableCollection<OrderStatusModel> _orderStatuses;
        public ReadOnlyObservableCollection<OrderStatusModel> OrderStatuses => _orderStatuses;

        [Reactive]
        public bool IsAddingOrderStatus { get; set; }

        [Reactive]
        public OrderStatusModel SelectedOrderStatus { get; set; }

        public ReactiveCommand<Unit, Unit> AddOrderStatusCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteOrderStatusCommand { get; }
        public ReactiveCommand<Unit, IEnumerable<OrderStatusModel>> LoadOrderStatuses { get; }

        #endregion Order Statuses

        #region Product Groups

        private readonly SourceCache<ProductGroupModel, int> _productGroupsSource;
        private readonly ReadOnlyObservableCollection<ProductGroupModel> _productGroups;
        public ReadOnlyObservableCollection<ProductGroupModel> ProductGroups => _productGroups;

        [Reactive]
        public bool IsAddingProductGroup { get; set; }

        [Reactive]
        public ProductGroupModel SelectedProductGroup { get; set; }

        public ReactiveCommand<Unit, Unit> AddProductGroupCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteProductGroupCommand { get; }
        public ReactiveCommand<Unit, IEnumerable<ProductGroupModel>> LoadProductGroups { get; }

        #endregion Product Groups

        #region Product Statuses

        private readonly SourceCache<ProductStatusModel, int> _productStatusesSource;
        private readonly ReadOnlyObservableCollection<ProductStatusModel> _productStatuses;
        public ReadOnlyObservableCollection<ProductStatusModel> ProductStatuses => _productStatuses;

        [Reactive]
        public bool IsAddingProductStatus { get; set; }

        [Reactive]
        public ProductStatusModel SelectedProductStatus { get; set; }

        public ReactiveCommand<Unit, Unit> AddProductStatusCommand { get; }
        public ReactiveCommand<Unit, IEnumerable<ProductStatusModel>> LoadProductStatuses { get; }
        public ReactiveCommand<Unit, Unit> DeleteProductStatusCommand { get; }

        #endregion Product Statuses

        [Reactive]
        public string AddingObjectName { get; set; }

        public ReactiveCommand<Unit, Unit> AddObjectCommand { get; }

        public ReactiveCommand<Unit, Unit> CancelAddingNewObjectCommand { get; }

        public CommonCataloguesViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;

            Activator = new ViewModelActivator();

            _productsService = Locator.Current.GetService<IProductsService>();
            _ordersService = Locator.Current.GetService<IOrdersService>();

            _orderStatusesSource = new SourceCache<OrderStatusModel, int>(x => x.Id);
            _productGroupsSource = new SourceCache<ProductGroupModel, int>(x => x.Id);
            _productStatusesSource = new SourceCache<ProductStatusModel, int>(x => x.Id);

            _orderStatusesSource
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _orderStatuses)
                .DisposeMany()
                .Subscribe();

            _productGroupsSource
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _productGroups)
                .DisposeMany()
                .Subscribe();

            _productStatusesSource
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _productStatuses)
                .DisposeMany()
                .Subscribe();

            LoadOrderStatuses = ReactiveCommand.CreateFromTask(() =>
            {
                _orderStatusesSource.Clear();

                return _ordersService.GetOrderStatusesAsync();
            });

            var canDeleteOrderStatus = this.WhenAnyValue(x => x.SelectedOrderStatus)
                .Select(x => x != null);

            DeleteOrderStatusCommand = ReactiveCommand.CreateFromTask(() =>
                _ordersService.DeleteOrderStatusAsync(SelectedOrderStatus)
                    .ContinueWith(_ =>
                    {
                        LoadOrderStatuses
                            .Execute()
                            .Subscribe(statuses => _orderStatusesSource.AddOrUpdate(statuses));
                    }),
                canExecute: canDeleteOrderStatus);

            LoadProductGroups = ReactiveCommand.CreateFromTask(() =>
            {
                _productGroupsSource.Clear();

                return _productsService.GetProductGroupsAsync();
            });

            var canDeleteProductGroup = this.WhenAnyValue(x => x.SelectedProductGroup)
                .Select(x => x != null);

            DeleteProductGroupCommand = ReactiveCommand.CreateFromTask(() =>
                _productsService.DeleteProductGroupAsync(SelectedProductGroup)
                    .ContinueWith(_ =>
                    {
                        LoadProductGroups
                            .Execute()
                            .Subscribe(groups => _productGroupsSource.AddOrUpdate(groups));
                    }),
                canExecute: canDeleteProductGroup);

            LoadProductStatuses = ReactiveCommand.CreateFromTask(() =>
            {
                _productStatusesSource.Clear();

                return _productsService.GetProductStatusesAsync();
            });

            var canDeleteProductStatus = this.WhenAnyValue(x => x.SelectedProductStatus)
                .Select(x => x != null);

            DeleteProductStatusCommand = ReactiveCommand.CreateFromTask(() =>
                _productsService.DeleteProductStatusAsync(SelectedProductStatus)
                    .ContinueWith(_ =>
                    {
                        LoadProductStatuses
                            .Execute()
                            .Subscribe(statuses => _productStatusesSource.AddOrUpdate(statuses));
                    }),
                canExecute: canDeleteProductStatus);

            AddOrderStatusCommand = ReactiveCommand.Create(() =>
            {
                IsAddingProductGroup = false;
                IsAddingProductStatus = false;
                IsAddingOrderStatus = true;
            });

            AddProductGroupCommand = ReactiveCommand.Create(() =>
            {
                IsAddingProductGroup = true;
                IsAddingProductStatus = false;
                IsAddingOrderStatus = false;
            });

            AddProductStatusCommand = ReactiveCommand.Create(() =>
            {
                IsAddingProductGroup = false;
                IsAddingProductStatus = true;
                IsAddingOrderStatus = false;
            });

            var canAddObject = this.WhenAnyValue(x => x.AddingObjectName)
                .Select(x => !string.IsNullOrWhiteSpace(x));

            AddObjectCommand = ReactiveCommand.CreateFromTask(() => AddNewObjectAsync(), canAddObject);

            CancelAddingNewObjectCommand = ReactiveCommand.Create(() =>
            {
                IsAddingProductGroup = false;
                IsAddingProductStatus = false;
                IsAddingOrderStatus = false;

                AddingObjectName = string.Empty;
            });

            this.WhenActivated(disposables =>
            {
                LoadProductGroups
                    .Execute()
                    .Subscribe(groups => _productGroupsSource.AddOrUpdate(groups))
                    .DisposeWith(disposables);

                LoadProductStatuses
                    .Execute()
                    .Subscribe(statuses => _productStatusesSource.AddOrUpdate(statuses))
                    .DisposeWith(disposables);

                LoadOrderStatuses
                    .Execute()
                    .Subscribe(statuses => _orderStatusesSource.AddOrUpdate(statuses))
                    .DisposeWith(disposables);

                Disposable.Create(() =>
                {
                    _orderStatusesSource.Clear();
                    _orderStatusesSource.Dispose();

                    _productGroupsSource.Clear();
                    _productGroupsSource.Dispose();

                    _productStatusesSource.Clear();
                    _productStatusesSource.Dispose();
                }).DisposeWith(disposables);
            });
        }

        private async Task AddNewObjectAsync()
        {
            if (IsAddingProductGroup)
            {
                var newProd = new ProductGroupModel()
                {
                    Name = AddingObjectName
                };

                await _productsService.AddProductGroupAsync(newProd);

                LoadProductGroups
                    .Execute()
                    .Subscribe(groups => _productGroupsSource.AddOrUpdate(groups));
            }
            else if (IsAddingProductStatus)
            {
                var newStatus = new ProductStatusModel()
                {
                    Name = AddingObjectName
                };

                await _productsService.AddProductStatusAsync(newStatus);

                LoadProductStatuses
                    .Execute()
                    .Subscribe(statuses => _productStatusesSource.AddOrUpdate(statuses));
            }
            else if (IsAddingOrderStatus)
            {
                var newStatus = new OrderStatusModel()
                {
                    Name = AddingObjectName
                };

                await _ordersService.AddOrderStatusAsync(newStatus);

                LoadOrderStatuses
                    .Execute()
                    .Subscribe(statuses => _orderStatusesSource.AddOrUpdate(statuses));
            }

            await CancelAddingNewObjectCommand.Execute();
        }
    }
}