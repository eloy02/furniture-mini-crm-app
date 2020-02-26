using ReactiveUI;
using Splat;

namespace FurnitureMiniCrm.App.Core.ViewModels
{
    public sealed class OrdersMainViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
    {
        public string UrlPathSegment { get; } = "/orders";
        public IScreen HostScreen { get; }
        public ViewModelActivator Activator { get; }

        public OrdersMainViewModel(IScreen hostScreen = null)
        {
            HostScreen = hostScreen;

            Activator = new ViewModelActivator();

            if (HostScreen is null)
                HostScreen = Locator.Current.GetService<IScreen>();
        }
    }
}