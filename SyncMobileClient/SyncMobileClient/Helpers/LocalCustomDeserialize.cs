using Newtonsoft.Json;
using SyncMobileClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncMobileClient.Helpers
{
    public class LocalCustomDeserialize : ICustomDeserialize
    {
        public ISyncItem Deserialize(string json, string tableName)
        {
            if (tableName == "UserPreference")
            {
                return JsonConvert.DeserializeObject<UserPreference>(json);
            }
            else if (tableName == "UserArticleStatus")
            {
                return JsonConvert.DeserializeObject<UserArticleStatus>(json);
            }
            else
            {
                throw new Exception(String.Format("Not a valid table name: {0}", tableName));
            }
        }

        public LocalCustomDeserialize() { }
    }
}
