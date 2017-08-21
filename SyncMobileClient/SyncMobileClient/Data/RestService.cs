using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using SyncMobileClient.Models;
using System.Diagnostics;
using Plugin.Connectivity;

namespace SyncMobileClient.Data
{
    public class RestService
    {
        private static HttpClient Client { get; set; }

        /// <summary>
        /// Calls Web Api to get Customer list in table storage
        /// </summary>
        /// <returns></returns>
        public static async Task<List<UserPreference>> GetPreferences()
        {
            Client = new HttpClient();

            try
            {
                string response = await Client.GetStringAsync("http://a00534e4.ngrok.io/api/Storage/Get");
                List<UserPreference> serverPreferenceList = (List<UserPreference>)JsonConvert.DeserializeObject<IEnumerable<UserPreference>>(response);

                return serverPreferenceList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return new List<UserPreference>();
            }
        }

        //-------------------------------------------- END GET REQUESTS ----------------------

        /// <summary>
        /// Send Post Request to Web Api to add Preference to Table storage
        /// </summary>
        public static async Task AddOrDeletePreference(UserPreference preference, string operation)
        {
            Client = new HttpClient();
            var preferencePayload = JsonConvert.SerializeObject(preference);
            var httpContent = new StringContent(preferencePayload, Encoding.UTF8, "application/json");
            string url = "";

            if (operation == "Add")
            {
                url = "http://a00534e4.ngrok.io/api/storage/addpreference";
            }
            else
            {
                url = "http://a00534e4.ngrok.io/api/storage/deletepreference";
            }

            try
            {
                HttpResponseMessage response = await Client.PostAsync(url, httpContent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //------------------------------------------- END POST REQUESTS ----------------------

        public static async Task UpdatePreference(UserPreference preference)
        {
            Client = new HttpClient();
            var preferencePayload = JsonConvert.SerializeObject(preference);
            var httpContent = new StringContent(preferencePayload, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await Client.PostAsync("http://a00534e4.ngrok.io/api/storage/updatepreference", httpContent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //------------------------------------------- END PUT REQUESTS ----------------------


        public static async Task SyncWithAzure()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                IEnumerable<CUD> operations = await PreferenceLocalDb.Instance.GetCUD();
                SyncTime lastSync = await PreferenceLocalDb.Instance.GetLastSync();
                SyncInfo clientSyncInfo = new SyncInfo(operations, lastSync.LastSyncTime);

                Client = new HttpClient();

                var SyncPayload = await Task.Run(() => JsonConvert.SerializeObject(clientSyncInfo));
                var httpContent = new StringContent(SyncPayload, Encoding.UTF8, "application/json");

                try
                {
                    var response = await Client.PostAsync("http://a00534e4.ngrok.io/api/storage/syncmobile", httpContent);
                    String responseContent = await response.Content.ReadAsStringAsync();
                    IEnumerable<CUD> operationsToPerform = JsonConvert.DeserializeObject<IEnumerable<CUD>>(responseContent);
                    await PreferenceLocalDb.Instance.SyncServerWithLocal(operationsToPerform);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
