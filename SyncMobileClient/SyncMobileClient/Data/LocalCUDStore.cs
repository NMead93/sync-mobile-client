using SyncMobileClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncMobileClient.Data
{
    public class LocalCUDStore : ICUDDataStore
    {
        public LocalCUDStore() { }

        public async Task<IEnumerable<ICUDEntity>> GetCUD()
        {
            return await LocalDb.Instance.GetCUD();
        }

        public async void AddCUDEntity(string tableName, string recordId, string customerId, string payload, string operation, DateTimeOffset operationDate)
        {
            CUD newCUD = new CUD(operation, tableName, payload, operationDate, customerId);

            await LocalDb.Instance.GetCUD();
        }

        public async void CleanUp(DateTimeOffset updatedSyncTime)
        {
            await LocalDb.Instance.ClearCUDTable();

            await LocalDb.Instance.SetLastSync(updatedSyncTime);
        }
    }
}
