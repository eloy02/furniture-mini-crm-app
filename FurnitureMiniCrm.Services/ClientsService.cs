using LiteDB;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace FurnitureMiniCrm.Services
{
    public class ClientsService : ServiceBase, IClientsService
    {
        private readonly Subject<ClientModel> _newClients = new Subject<ClientModel>();

        public Task<IEnumerable<ClientModel>> GetClientsAsync() => Task.FromResult(Get<ClientModel>());

        public Task<ClientModel> GetClientAsync(int id)
        {
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<ClientModel>();

            col.EnsureIndex(x => x.Id);

            var client = col.Query()
                .Where(x => x.Id == id)
                .SingleOrDefault();

            return Task.FromResult(client);
        }

        public Task SetClientAsync(ClientModel client)
        {
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<ClientModel>();

            col.EnsureIndex(x => x.Id);

            if (col.Exists(x => x.Id == client.Id))
                col.Update(client.Id, client);
            else
                col.Insert(client);

            _newClients.OnNext(client);

            return Task.CompletedTask;
        }

        public IObservable<ClientModel> NewClients => _newClients;
    }
}