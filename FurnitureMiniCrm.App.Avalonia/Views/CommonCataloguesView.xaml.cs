using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FurnitureMiniCrm.App.Core.ViewModels;
using ReactiveUI;

namespace AvaloniaAppTemplate.Namespace
{
    public class CommonCataloguesView : ReactiveUserControl<CommonCataloguesViewModel>
    {
        public Grid NewObjectBox => this.FindControl<Grid>("NewObjectBox");
        public TextBlock NewObjectTitle => this.FindControl<TextBlock>("NewObjectTitle");
        public TextBox NewObjectName => this.FindControl<TextBox>("NewObjectName");

        public CommonCataloguesView()
        {
            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x =>
                        x.ViewModel.IsAddingOrderStatus,
                        x => x.ViewModel.IsAddingProductGroup,
                        x => x.ViewModel.IsAddingProductStatus,
                        (isAddingOrderStatus, isAddingProductGroup, isAddingProductStatus) =>
                            isAddingOrderStatus || isAddingProductGroup || isAddingProductStatus)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(isAddingObject => NewObjectBox.IsVisible = isAddingObject)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.ViewModel.IsAddingOrderStatus)
                    .Where(x => x == true)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ => NewObjectTitle.Text = "Название нового статуса заказа")
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.ViewModel.IsAddingProductGroup)
                    .Where(x => x == true)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ => NewObjectTitle.Text = "Название новой группы товаров")
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.ViewModel.IsAddingProductStatus)
                    .Where(x => x == true)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(_ => NewObjectTitle.Text = "Название нового статуса товаров")
                    .DisposeWith(disposables);
            });

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}