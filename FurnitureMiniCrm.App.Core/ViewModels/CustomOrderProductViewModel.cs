using System;
using System.Reactive;
using FurnitureMiniCrm.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FurnitureMiniCrm.App.Core.ViewModels
{
    public sealed class CustomOrderProductViewModel : ReactiveObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; }

        public Action<CustomOrderProductModel> OnOrderProductCreated { get; set; }

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

        public ReactiveCommand<Unit, bool> AddProduct { get; }

        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public CustomOrderProductViewModel()
        {
            Activator = new ViewModelActivator();

            var canAdd = this.WhenAnyValue(
                x => x.ProductName,
                x => x.ProductUnit,
                x => x.ProductPrice,
                x => x.ProductSellPrice,
                (name, unit, price, sellPrice) =>
                    !string.IsNullOrWhiteSpace(name)
                    && !string.IsNullOrWhiteSpace(unit)
                    && price != null
                    && sellPrice != null);

            AddProduct = ReactiveCommand.Create(() =>
            {
                var prod = new CustomOrderProductModel()
                {
                    Name = ProductName,
                    Price = ProductPrice.Value,
                    SellPrice = ProductSellPrice.Value,
                    Size = Size,
                    Unit = ProductUnit,
                };

                OnOrderProductCreated?.Invoke(prod);

                return true;
            },
            canExecute: canAdd);

            Cancel = ReactiveCommand.Create(() => Unit.Default);

            //this.WhenActivated(disposables =>
            //{
            //});
        }
    }
}