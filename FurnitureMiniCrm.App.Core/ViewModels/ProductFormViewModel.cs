using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using FurnitureMiniCrm.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace FurnitureMiniCrm.App.Core.ViewModels
{
    public class ProductFormViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        private readonly IProductsService _productsService;

        public string UrlPathSegment => "/products/form";

        public IScreen HostScreen { get; }

        public ViewModelActivator Activator { get; }

        [Reactive]
        public ObservableCollection<ProductGroupModel> ProductGroups { get; set; }

        [Reactive]
        public ProductGroupModel SelectedProductGroup { get; set; }

        [Reactive]
        public ObservableCollection<ProductStatusModel> ProductStatuses { get; set; }

        [Reactive]
        public ProductStatusModel SelectedProductStatus { get; set; }

        [Reactive]
        public bool IsEditingMode { get; set; } = false;

        [Reactive]
        public string ProductCode { get; set; }

        [Reactive]
        public string ProductName { get; set; }

        [Reactive]
        public string ProductUnit { get; set; }

        [Reactive]
        public string Size { get; set; }

        [Reactive]
        public double? ProductPrice { get; set; }

        [Reactive]
        public double? ProductSellPrice { get; set; }

        public ReactiveCommand<Unit, Unit> SaveProduct { get; }

        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public ProductFormViewModel(IScreen hostScreen = null, ProductModel productForEdit = null)
        {
            HostScreen = hostScreen;

            if (HostScreen is null)
            {
                HostScreen = Locator.Current.GetService<IScreen>();
            }

            _productsService = Locator.Current.GetService<IProductsService>();

            Activator = new ViewModelActivator();

            if (productForEdit != null)
            {
                IsEditingMode = true;
            }

            IObservable<bool> canSaveProduct = this.WhenAnyValue(
                x => x.ProductName,
                x => x.ProductCode,
                x => x.ProductUnit,
                x => x.ProductPrice,
                x => x.ProductSellPrice,
                x => x.SelectedProductStatus,
                x => x.SelectedProductGroup,
                (name, code, unit, price, sellPrice, status, group) =>
                    !string.IsNullOrWhiteSpace(name)
                    && !string.IsNullOrWhiteSpace(code)
                    && !string.IsNullOrWhiteSpace(unit)
                    && price != null
                    && sellPrice != null
                    && status != null
                    && group != null);

            SaveProduct = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!IsEditingMode)
                {
                    await _productsService.SetProductAsync(new ProductModel()
                    {
                        CodeName = ProductCode,
                        Group = SelectedProductGroup,
                        Name = ProductName,
                        Price = ProductPrice.Value,
                        SellPrice = ProductSellPrice.Value,
                        Status = SelectedProductStatus,
                        Unit = ProductUnit,
                        Size = Size
                    });
                }
                else
                {
                    await _productsService.SetProductAsync(new ProductModel()
                    {
                        Id = productForEdit.Id,
                        CodeName = ProductCode,
                        Group = SelectedProductGroup,
                        Name = ProductName,
                        Price = ProductPrice.Value,
                        SellPrice = ProductSellPrice.Value,
                        Status = SelectedProductStatus,
                        Unit = ProductUnit,
                        Size = Size
                    });
                }

                await hostScreen.Router.NavigateBack.Execute();
            }, canSaveProduct);

            Cancel = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.NavigateBack.Execute());

            ReactiveCommand<Unit, System.Collections.Generic.IEnumerable<ProductStatusModel>> loadStatuses = ReactiveCommand.CreateFromTask(() => _productsService.GetProductStatusesAsync());

            ReactiveCommand<Unit, System.Collections.Generic.IEnumerable<ProductGroupModel>> loadGroups = ReactiveCommand.CreateFromTask(() => _productsService.GetProductGroupsAsync());

            this.WhenActivated(disposables =>
            {
                loadGroups
                    .Execute()
                    .Select(groups => new ObservableCollection<ProductGroupModel>(groups))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(groups => ProductGroups = groups)
                    .DisposeWith(disposables);

                loadStatuses
                    .Execute()
                    .Select(statuses => new ObservableCollection<ProductStatusModel>(statuses))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(statuses => ProductStatuses = statuses)
                    .DisposeWith(disposables);

                if (!IsEditingMode)
                {
                    this.WhenAnyValue(x => x.ProductStatuses)
                        .Where(statuses => statuses != null)
                        .Select(statuses => statuses.FirstOrDefault())
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(status => SelectedProductStatus = status)
                        .DisposeWith(disposables);

                    this.WhenAnyValue(x => x.ProductGroups)
                        .Where(groups => groups != null)
                        .Select(groups => groups.FirstOrDefault())
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(group => SelectedProductGroup = group)
                        .DisposeWith(disposables);
                }
                else
                {
                    this.WhenAnyValue(x => x.ProductStatuses)
                        .Where(statuses => statuses != null)
                        .Select(statuses => statuses.SingleOrDefault(s => s.Id == productForEdit.Status.Id))
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(status => SelectedProductStatus = status)
                        .DisposeWith(disposables);

                    this.WhenAnyValue(x => x.ProductGroups)
                        .Where(groups => groups != null)
                        .Select(groups => groups.SingleOrDefault(s => s.Id == productForEdit.Group.Id))
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(group => SelectedProductGroup = group)
                        .DisposeWith(disposables);

                    ProductCode = productForEdit.CodeName;
                    ProductName = productForEdit.Name;
                    ProductPrice = productForEdit.Price;
                    ProductSellPrice = productForEdit.SellPrice;
                    ProductUnit = productForEdit.Unit;
                }
            });
        }
    }
}