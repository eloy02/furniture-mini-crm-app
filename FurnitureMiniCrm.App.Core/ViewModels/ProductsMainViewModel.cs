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
    public sealed class ProductsMainViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
    {
        private readonly IProductsService _productsService;
        private readonly SourceCache<ProductModel, int> _productsSource;

        private readonly ReadOnlyObservableCollection<ProductModel> _products;

        public string UrlPathSegment { get; } = "/products";
        public IScreen HostScreen { get; }
        public ViewModelActivator Activator { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> CreateProduct { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> EditProduct { get; }
        public ReadOnlyObservableCollection<ProductModel> Products => _products;

        [Reactive]
        public ProductModel SelectedProduct { get; set; }

        public ProductsMainViewModel(IScreen hostScreen = null)
        {
            _productsService = Locator.Current.GetService<IProductsService>();

            HostScreen = hostScreen;

            if (HostScreen is null)
            {
                HostScreen = Locator.Current.GetService<IScreen>();
            }

            Activator = new ViewModelActivator();

            _productsSource = new SourceCache<ProductModel, int>(x => x.Id);

            CreateProduct = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new ProductFormViewModel(HostScreen)));

            var canEditProduct = this.WhenAnyValue(x => x.SelectedProduct)
               .Select(product => product != null);

            EditProduct = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new ProductFormViewModel(HostScreen, SelectedProduct)), canEditProduct);

            var loadProducts = ReactiveCommand.CreateFromTask(async () => await _productsService.GetProductsAsync());

            _productsSource
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _products)
                .DisposeMany()
                .Subscribe();

            this.WhenActivated(disposables =>
            {
                Disposable.Create(() => _productsSource.Clear())
                    .DisposeWith(disposables);

                _productsSource.PopulateFrom(loadProducts.Execute());

                _productsService.NewProducts
                    .Where(product => product != null)
                    .Subscribe(product => _productsSource.AddOrUpdate(product))
                    .DisposeWith(disposables);
            });
        }
    }
}