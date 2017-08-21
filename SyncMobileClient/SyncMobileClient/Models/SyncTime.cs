using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncMobileClient.Models
{
    [Table("SyncTime")]
    public class SyncTime
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTimeOffset LastSyncTime { get; set; }

        public SyncTime() { }

        public SyncTime(DateTimeOffset syncTime)
        {
            LastSyncTime = syncTime;
        }
    }
}
