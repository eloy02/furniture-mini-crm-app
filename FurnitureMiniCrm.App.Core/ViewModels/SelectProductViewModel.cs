using ReactiveUI;

namespace FurnitureMiniCrm.App.Core.ViewModels
{
    public sealed class SelectProductViewModel : ReactiveObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; }
    }
}