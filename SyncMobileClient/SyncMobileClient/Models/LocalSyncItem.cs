using SyncMobileClient.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncMobileClient.Models
{
    public abstract class LocalSyncItem : ISyncItem
    {
        public string TableName { get; set; }

        public string CustomerId { get; set; }

        public async Task Create()
        {
            await LocalDb.Instance.AddSyncItem(this);
        }

        public async Task<IEnumerable<ISyncItem>> GetAll()
        {
            return await LocalDb.Instance.GetAll(TableName);
        }

        public async Task Update()
        {
            await UpdateImp();
        }

        public async Task Delete()
        {
            await LocalDb.Instance.DeleteSyncItem(this);
        }

        public bool IsEqual(ISyncItem otherItem)
        {
            return IsEqualImp(otherItem);
        }

        public async Task<ISyncItem> FindLatestItemCopy()
        {
            return await GetLocalItemCopyImp();
        }

        protected abstract bool IsEqualImp(ISyncItem otherItem);

        protected abstract Task UpdateImp();

        protected abstract Task<ISyncItem> GetLocalItemCopyImp();
    }
}
