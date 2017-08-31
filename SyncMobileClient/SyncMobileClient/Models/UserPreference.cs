using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SyncMobileClient.Data;

namespace SyncMobileClient.Models
{
    [Table("UserPreference")]
    public class UserPreference : LocalSyncItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
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

        protected override bool IsEqualImp(ISyncItem otherItem)
        {
            UserPreference castedOtherItem = (UserPreference)otherItem;
            return (PreferenceName == castedOtherItem.PreferenceName) && (PreferenceValue == castedOtherItem.PreferenceValue);
        }

        protected override async Task UpdateImp()
        {
            UserPreference queriedPreference = await LocalDb.Instance.GetPreference(PreferenceName);
            queriedPreference.PreferenceValue = PreferenceValue;

            await LocalDb.Instance.UpdateSyncItem(this);
        }

        protected override async Task<ISyncItem> GetLocalItemCopyImp()
        {
            return await LocalDb.Instance.GetPreference(PreferenceName);
        }
    }
}
