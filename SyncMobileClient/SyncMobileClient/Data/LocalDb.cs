using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SQLite;
using SyncMobileClient.Models;
using System.Diagnostics;
using Plugin.Connectivity;
using Newtonsoft.Json;

namespace SyncMobileClient.Data
{
    public class LocalDb
    {
        private SQLiteAsyncConnection conn;

        // Singleton of the database repository object.
        private static LocalDb instance;
        public static LocalDb Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("You must call Initialize before retrieving the singleton for the PreferenceLocalDb.");
                }

                return instance;
            }
        }

        public static void Initialize(string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException(nameof(filename));
            }

            //avoid db Locking and free unmanaged resources
            if (instance != null)
            {
                instance.conn.GetConnection().Dispose();
            }

            instance = new LocalDb(filename);
        }

        private LocalDb(string dbPath)
        {
            try
            {
                conn = new SQLiteAsyncConnection(dbPath);
                conn.CreateTableAsync<UserPreference>().Wait();
                conn.CreateTableAsync<UserArticleStatus>().Wait();
                conn.CreateTableAsync<CUD>().Wait();
                conn.CreateTableAsync<SyncTime>().Wait();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task<IEnumerable<ISyncItem>> GetAll(string tableName)
        {
            try
            {
                if (tableName == "UserPreference")
                {
                    return await conn.Table<UserPreference>().ToListAsync();
                }
                else
                {
                    return await conn.Table<UserArticleStatus>().ToListAsync();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Failed to retrieve data. {0}", ex.InnerException.Message));
            }

            return Enumerable.Empty<ISyncItem>();
        }

        public async Task<UserPreference> GetPreference(string preferenceName)
        {
            return await conn.Table<UserPreference>().Where(i => i.PreferenceName == preferenceName).FirstOrDefaultAsync();
        }

        public async Task<UserArticleStatus> GetUserArticleStatus(string articleCode)
        {
            return await conn.Table<UserArticleStatus>().Where(i => i.ArticleCode == articleCode).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CUD>> GetCUD()
        {
            try
            {
                IEnumerable<CUD> offlineOperations = await conn.Table<CUD>().ToListAsync();

                return offlineOperations;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return null;
            }
        }

        public async Task<SyncTime> GetLastSync()
        {
            List<SyncTime> lastSync = await conn.Table<SyncTime>().ToListAsync();

            if (lastSync.Count == 0)
            {
                SyncTime newSyncTime = new SyncTime(DateTimeOffset.MinValue);
                int result = await conn.InsertAsync(newSyncTime);

                return newSyncTime;
            }

            return lastSync[0];
        }

        //--------------------------END GET METHODS-----------------

        public async Task AddSyncItem(ISyncItem item)
        {
            try
            {
                int result = await conn.InsertAsync(item);
                CUD newCUD = new CUD("Create", item.TableName, JsonConvert.SerializeObject(item), DateTimeOffset.Now, "Customer");
                await AddCUD(newCUD);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task AddCUD(CUD operation)
        {
            try
            {
                int result = await conn.InsertAsync(operation);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //---------------------END POST METHODS---------------

        public async Task UpdateSyncItem(ISyncItem updatedItem)
        {
            await conn.UpdateAsync(updatedItem);
            CUD newCUD = new CUD("Update", updatedItem.TableName, JsonConvert.SerializeObject(updatedItem), DateTimeOffset.Now, "Customer");
            await AddCUD(newCUD);
        }

        public async Task SetLastSync(DateTimeOffset updatedSyncTime)
        {
            SyncTime lastSync = await GetLastSync();

            lastSync.LastSyncTime = updatedSyncTime;
            await conn.UpdateAsync(lastSync);

        }

        public async Task ExpireLastSync()
        {
            SyncTime lastSync = await GetLastSync();

            //2 months old
            lastSync.LastSyncTime = DateTimeOffset.Now.AddMonths(-2);
            await conn.UpdateAsync(lastSync);
        }

        //====================== END PUT METHODS ====================

        public async Task DeleteSyncItem(ISyncItem item)
        {
            await conn.DeleteAsync(item);
            CUD newCUD = new CUD("Delete", item.TableName, JsonConvert.SerializeObject(item), DateTimeOffset.Now, "Customer");
            await AddCUD(newCUD);
        }

        public async Task NukeTables()
        {
            try
            {
                await conn.DropTableAsync<CUD>();
                await conn.DropTableAsync<UserPreference>();
                await conn.DropTableAsync<SyncTime>();
                await conn.CreateTableAsync<UserPreference>();
                await conn.CreateTableAsync<CUD>();
                await conn.CreateTableAsync<SyncTime>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task ClearCUDTable()
        {
            try
            {
                await conn.DropTableAsync<CUD>();
                await conn.CreateTableAsync<CUD>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could Not Clear CUD Table: " + ex.Message);
            }
        }

        //=======================  END DELETE METHODS ================

        //public async Task SyncServerWithLocal(IEnumerable<CUD> serverChanges)
        //{
        //    foreach(CUD operation in serverChanges)
        //    {
        //        UserPreference operationPayload = JsonConvert.DeserializeObject<UserPreference>(operation.Payload);
        //        IEnumerable<UserPreference> localPreferences = await GetAll();
        //        UserPreference localQuery = localPreferences.Where(x => x.PreferenceName == operationPayload.PreferenceName).FirstOrDefault();

        //        if (operation.Operation == "Create")
        //        {
        //            if (localQuery == null)
        //            {
        //                await AddPreference(operationPayload);
        //            }
        //            if (localQuery != null && localQuery.PreferenceValue != operationPayload.PreferenceValue)
        //            {
        //                await UpdatePreference(operationPayload.PreferenceName, operationPayload.PreferenceValue);
        //            }
        //        }
        //        else if (operation.Operation == "Update")
        //        {
        //            if (localQuery == null)
        //            {
        //                await AddPreference(operationPayload);
        //            }
        //            if (localQuery != null && localQuery.PreferenceValue != operationPayload.PreferenceValue)
        //            {
        //                await UpdatePreference(operationPayload.PreferenceName, operationPayload.PreferenceValue);
        //            }
        //        }
        //        else
        //        {
        //            if (localQuery != null)
        //            {
        //                await DeletePreference(operationPayload.PreferenceName);
        //            }
        //        }
        //    }

        //    //cleanup
        //    await SetLastSync();
        //    await ClearCUDTable();
        //}
    }
}
