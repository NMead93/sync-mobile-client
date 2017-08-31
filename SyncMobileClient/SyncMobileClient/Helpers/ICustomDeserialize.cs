using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncMobileClient.Models;

namespace SyncMobileClient.Helpers
{
    public interface ICustomDeserialize
    {
        ISyncItem Deserialize(string json, string tableName);
    }
}
