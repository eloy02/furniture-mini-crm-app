using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FurnitureMiniCrm.App.Core.ViewModels
{
    public class OrderFormViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
    {
        public string UrlPathSegment { get; } = "/orders/form";
        public IScreen HostScreen { get; }
        public ViewModelActivator Activator { get; }

        public ReactiveCommand<Unit, Unit> SelectClientCommand { get; }
        public ReactiveCommand<Unit, Unit> SelectProductCommand { get; }

        public Interaction<SelectClientViewModel, Unit> SelectClient { get; }
        public Interaction<SelectProductViewModel, Unit> SelectProduct { get; }

        public OrderFormViewModel(IScreen hostScreen = null)
        {
            HostScreen = hostScreen;

            if (HostScreen is null)
                HostScreen = Locator.Current.GetService<IScreen>();

            Activator = new ViewModelActivator();

            SelectClient = new Interaction<SelectClientViewModel, Unit>();
            SelectProduct = new Interaction<SelectProductViewModel, Unit>();

            SelectClientCommand = ReactiveCommand.CreateFromTask(() => SelectClientAsync());
            SelectProductCommand = ReactiveCommand.CreateFromTask(() => SelectProductAsync());
        }

        private async Task SelectProductAsync()
        {
            var selectProductVm = new SelectProductViewModel();

            await SelectProduct.Handle(selectProductVm);

            // todo get selected product
        }

        private async Task SelectClientAsync()
        {
            var selectClientVm = new SelectClientViewModel();

            await SelectClient.Handle(selectClientVm);

            //todo get selected client
        }
    }
}