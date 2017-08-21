using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SyncMobileClient.Models;
using System.Diagnostics;
using Plugin.Connectivity;
using Newtonsoft.Json;

namespace SyncMobileClient.Data
{
    public class PreferenceLocalDb
    {
        private SQLiteAsyncConnection conn;

        // Singleton of the database repository object.
        private static PreferenceLocalDb instance;
        public static PreferenceLocalDb Instance
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

            instance = new PreferenceLocalDb(filename);
        }

        private PreferenceLocalDb(string dbPath)
        {
            try
            {
                conn = new SQLiteAsyncConnection(dbPath);
                conn.CreateTableAsync<UserPreference>().Wait();
                conn.CreateTableAsync<CUD>().Wait();
                conn.CreateTableAsync<SyncTime>().Wait();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task<IEnumerable<UserPreference>> GetAllPreferences()
        {
            try
            {
                return await conn.Table<UserPreference>().ToListAsync();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Failed to retrieve data. {0}", ex.InnerException.Message));
            }

            return Enumerable.Empty<UserPreference>();
        }

        public async Task<UserPreference> GetPreference(string preferenceName)
        {
            return await conn.Table<UserPreference>().Where(i => i.PreferenceName == preferenceName).FirstOrDefaultAsync();
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

        public async Task AddPreference(UserPreference preference)
        {
            try
            {
                int result = await conn.InsertAsync(preference);

                if (!CrossConnectivity.Current.IsConnected)
                {
                    CUD newCUD = new CUD("Create", preference.TableName, JsonConvert.SerializeObject(preference), DateTimeOffset.Now);
                    await AddCUD(newCUD);
                }
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

        public async Task UpdatePreference(string preferenceName, string updatedValue)
        {
            UserPreference storedPreference = await GetPreference(preferenceName);
            storedPreference.PreferenceValue = updatedValue;
            await conn.UpdateAsync(storedPreference);

            if (!CrossConnectivity.Current.IsConnected)
            {
                CUD newCUD = new CUD("Update", storedPreference.TableName, JsonConvert.SerializeObject(storedPreference), DateTimeOffset.Now);
                await AddCUD(newCUD);
            }
        }

        public async Task SetLastSync()
        {
            SyncTime lastSync = await GetLastSync();

            lastSync.LastSyncTime = DateTimeOffset.Now;
            await conn.UpdateAsync(lastSync);

        }

        //====================== END PUT METHODS ====================

        public async Task DeletePreference(string preferenceName)
        {
            UserPreference storedPreference = await GetPreference(preferenceName);
            await conn.DeleteAsync(storedPreference);

            if (!CrossConnectivity.Current.IsConnected)
            {
                CUD newCUD = new CUD("Delete", storedPreference.TableName, JsonConvert.SerializeObject(storedPreference), DateTimeOffset.Now);
                await AddCUD(newCUD);
            }
        }

        public async Task NukeTables()
        {
            try
            {
                await conn.DropTableAsync<CUD>();
                await conn.DropTableAsync<UserPreference>();
                await conn.DropTableAsync<SyncTime>();
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could Not Clear CUD Table: " + ex.Message);
            }
        }

        //=======================  END DELETE METHODS ================

        public async Task SyncServerWithLocal(IEnumerable<CUD> serverChanges)
        {
            foreach(CUD operation in serverChanges)
            {
                UserPreference operationPayload = JsonConvert.DeserializeObject<UserPreference>(operation.Payload);
                IEnumerable<UserPreference> localPreferences = await GetAllPreferences();
                UserPreference localQuery = localPreferences.Where(x => x.PreferenceName == operationPayload.PreferenceName).FirstOrDefault();

                if (operation.Operation == "Create")
                {
                    if (localQuery == null)
                    {
                        await AddPreference(operationPayload);
                    }
                    if (localQuery != null && localQuery.PreferenceValue != operationPayload.PreferenceValue)
                    {
                        await UpdatePreference(operationPayload.PreferenceName, operationPayload.PreferenceValue);
                    }
                }
                else if (operation.Operation == "Update")
                {
                    if (localQuery == null)
                    {
                        await AddPreference(operationPayload);
                    }
                    if (localQuery != null && localQuery.PreferenceValue != operationPayload.PreferenceValue)
                    {
                        await UpdatePreference(operationPayload.PreferenceName, operationPayload.PreferenceValue);
                    }
                }
                else
                {
                    if (localQuery != null)
                    {
                        await DeletePreference(operationPayload.PreferenceName);
                    }
                }
            }

            //cleanup
            await SetLastSync();
            await ClearCUDTable();
        }
    }
}
