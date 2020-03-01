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
    public sealed class SelectProductViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly IProductsService _productsService;

        public ViewModelActivator Activator { get; }

        [Reactive]
        public string CodeName { get; set; }

        [Reactive]
        public string Name { get; set; }

        [Reactive]
        public ObservableCollection<ProductGroupModel> ProductGroups { get; set; }

        [Reactive]
        public ProductGroupModel SelectedProductGroup { get; set; }

        [Reactive]
        public ObservableCollection<ProductModel> Products { get; set; }

        [Reactive]
        public ProductModel SelectedProduct { get; set; }

        public ReactiveCommand<Unit, Unit> SelectProduct { get; }

        public Interaction<bool, Unit> Close { get; }

        public SelectProductViewModel()
        {
            Activator = new ViewModelActivator();

            _productsService = Locator.Current.GetService<IProductsService>();

            Close = new Interaction<bool, Unit>();

            var canChooseProduct = this.WhenAnyValue(x => x.SelectedProduct)
                .Select(product => product != null);
            SelectProduct = ReactiveCommand.CreateFromObservable(() => Close.Handle(true), canChooseProduct);

            var loadGroups = ReactiveCommand.CreateFromTask(async () => await _productsService.GetProductGroupsAsync());
            var loadProducts = ReactiveCommand.CreateFromTask(async () => await _productsService.GetProductsAsync());

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.CodeName, x => x.Name, x => x.SelectedProductGroup)
                    .SelectMany(_ => loadProducts.Execute())
                    .Select(products => FilterProducts(products))
                    .Select(products => new ObservableCollection<ProductModel>(products))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(products => Products = products)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.ProductGroups)
                    .Where(groups => groups != null)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(groups => SelectedProductGroup = groups.FirstOrDefault());

                loadGroups
                    .Execute()
                    .Select(groups => new ObservableCollection<ProductGroupModel>(groups))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(groups => ProductGroups = groups);

                loadProducts
                    .Execute()
                    .Select(products => FilterProducts(products))
                    .Select(products => new ObservableCollection<ProductModel>(products))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(products => Products = products)
                    .DisposeWith(disposables);
            });
        }

        private IEnumerable<ProductModel> FilterProducts(IEnumerable<ProductModel> products)
        {
            if (SelectedProductGroup != null)
                products = products.Where(p => p.Group.Id == SelectedProductGroup.Id);

            if (!string.IsNullOrWhiteSpace(CodeName))
                products = products.Where(p => p.CodeName.ToLower().Contains(CodeName.ToLower()));

            if (!string.IsNullOrWhiteSpace(Name))
                products = products.Where(p => p.Name.ToLower().Contains(CodeName.ToLower()));

            return products;
        }
    }
}