using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SyncMobileClient.Models
{
    [Table("UserPreferences")]
    public class UserPreference
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string TableName { get; set; }
        public string CustomerId { get; set; }
        public string PreferenceName { get; set; }
        public string PreferenceValue { get; set; }

        public UserPreference() { }

        public UserPreference(string customerId, string preferenceName, string preferenceValue)
        {
            TableName = "UserPreference";
            CustomerId = customerId;
            PreferenceName = preferenceName;
            PreferenceValue = preferenceValue;
        }
    }
}
