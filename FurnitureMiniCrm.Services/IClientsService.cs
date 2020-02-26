using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurnitureMiniCrm.Services
{
    public interface IClientsService
    {
        Task<IEnumerable<ClientModel>> GetClientsAsync();

        Task<ClientModel> GetClientAsync(int id);

        Task SetClientAsync(ClientModel client);

        IObservable<ClientModel> NewClients { get; }
    }
}