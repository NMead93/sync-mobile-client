using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncMobileClient.Models
{
    public class SyncInfo
    {
        public IEnumerable<CUD> ClientOperations { get; set; }
        public DateTimeOffset LastSync { get; set; }

        public SyncInfo(IEnumerable<CUD> operations, DateTimeOffset lastSync)
        {
            ClientOperations = operations;
            LastSync = lastSync;
        }
    }
}
