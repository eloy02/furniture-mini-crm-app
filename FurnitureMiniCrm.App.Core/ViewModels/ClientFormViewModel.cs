using System;
using System.Reactive;
using System.Reactive.Disposables;
using FurnitureMiniCrm.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace FurnitureMiniCrm.App.Core.ViewModels
{
    public class ClientFormViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
    {
        private readonly IClientsService _clientsService;

        public ViewModelActivator Activator { get; }
        public string UrlPathSegment => "/clients/form";
        public IScreen HostScreen { get; }

        [Reactive]
        public string LastName { get; set; }

        [Reactive]
        public string FirstName { get; set; }

        [Reactive]
        public string PatronymicName { get; set; }

        [Reactive]
        public string City { get; set; }

        [Reactive]
        public string Street { get; set; }

        [Reactive]
        public string Building { get; set; }

        [Reactive]
        public string Flat { get; set; }

        [Reactive]
        public string PhoneNumber { get; set; }

        [Reactive]
        public string Email { get; set; }

        [Reactive]
        public string Comments { get; set; }

        [Reactive]
        public bool IsEditMode { get; set; } = false;

        public ReactiveCommand<Unit, Unit> SaveClient { get; }

        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public ClientFormViewModel(IScreen hostScreen, ClientModel clientForEdit = null)
        {
            HostScreen = hostScreen;

            Activator = new ViewModelActivator();

            _clientsService = Locator.Current.GetService<IClientsService>();

            IsEditMode = clientForEdit != null;

            var canSaveClient = this.WhenAnyValue(
                x => x.FirstName,
                x => x.LastName,
                x => x.PhoneNumber,
                x => x.City,
                x => x.Street,
                x => x.Building,
                (firstName, lastName, phoneNumber, city, street, building) =>
                 !string.IsNullOrWhiteSpace(firstName)
                 && !string.IsNullOrWhiteSpace(lastName)
                 && !string.IsNullOrWhiteSpace(phoneNumber)
                 && !string.IsNullOrWhiteSpace(city)
                 && !string.IsNullOrWhiteSpace(street)
                 && !string.IsNullOrWhiteSpace(building));

            SaveClient = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!IsEditMode)
                {
                    await _clientsService.SetClientAsync(new ClientModel()
                    {
                        LastName = LastName,
                        FirstName = FirstName,
                        PatronymicName = PatronymicName,
                        Comments = Comments,
                        EmailAddress = Email,
                        PhoneNumber = PhoneNumber,
                        Address = new ClientAddressModel()
                        {
                            Street = Street,
                            City = City,
                            BuildingNumber = Building,
                            FlatNumber = Flat
                        }
                    });
                }
                else
                {
                    await _clientsService.SetClientAsync(new ClientModel()
                    {
                        Id = clientForEdit.Id,
                        LastName = LastName,
                        FirstName = FirstName,
                        PatronymicName = PatronymicName,
                        Comments = Comments,
                        EmailAddress = Email,
                        PhoneNumber = PhoneNumber,
                        Address = new ClientAddressModel()
                        {
                            Street = Street,
                            City = City,
                            BuildingNumber = Building,
                            FlatNumber = Flat
                        }
                    });
                }

                HostScreen.Router.NavigateBack.Execute();
            }, canSaveClient);

            Cancel = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.NavigateBack.Execute());

            var loadClientForEdit = ReactiveCommand.CreateFromTask(async () => await _clientsService.GetClientAsync(clientForEdit.Id));

            this.WhenActivated(disposables =>
            {
                if (IsEditMode)
                {
                    loadClientForEdit
                        .Execute()
                        .Subscribe(client =>
                        {
                            clientForEdit = client;

                            LastName = clientForEdit.LastName;
                            FirstName = clientForEdit.FirstName;
                            PatronymicName = clientForEdit.PatronymicName;
                            PhoneNumber = clientForEdit.PhoneNumber;
                            Email = clientForEdit.EmailAddress;
                            Comments = clientForEdit.Comments;

                            City = clientForEdit.Address.City;
                            Street = clientForEdit.Address.Street;
                            Building = clientForEdit.Address.BuildingNumber;
                            Flat = clientForEdit.Address.FlatNumber;
                        })
                        .DisposeWith(disposables);
                }
            });
        }
    }
}