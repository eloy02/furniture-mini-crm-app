using ReactiveUI;

namespace FurnitureMiniCrm.App.Core.ViewModels
{
    public sealed class SelectClientViewModel : ReactiveObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; }

        public SelectClientViewModel()
        {
            Activator = new ViewModelActivator();
        }
    }
}