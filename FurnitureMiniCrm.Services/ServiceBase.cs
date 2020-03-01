using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;

namespace FurnitureMiniCrm.Services
{
    public class ServiceBase
    {
        private protected readonly string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "furniturecrm",
            "database.db");

        private protected IEnumerable<T> Get<T>() where T : class
        {
            using var db = new LiteDatabase(dbPath);

            var col = db.GetCollection<T>();

            var res = col.FindAll();

            return res.ToList();
        }
    }
}