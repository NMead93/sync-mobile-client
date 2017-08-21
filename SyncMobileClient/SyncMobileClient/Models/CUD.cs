using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncMobileClient.Models
{
    [Table("CUD")]
    public class CUD
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Operation { get; set; }
        public string CustomerId { get; set; }
        public string TableName { get; set; }
        public string Payload { get; set; }
        public DateTimeOffset OperationTime { get; set; }

        public CUD() { }

        public CUD (string operation, string tableName, string payload, DateTimeOffset date)
        {
            Operation = operation;
            TableName = tableName;
            Payload = payload;
            OperationTime = date;
            CustomerId = "Customer";
        }
    }
}
