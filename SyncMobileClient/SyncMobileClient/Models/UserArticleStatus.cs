using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncMobileClient.SyncEnums;
using SQLite;
using SyncMobileClient.Data;

namespace SyncMobileClient.Models
{
    [Table("UserArticleStatus")]
    public class UserArticleStatus : LocalSyncItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string ArticleCode { get; set; }

        public Enums.ArticleStatus ArticleStatus { get; set; }

        public UserArticleStatus() { }

        public UserArticleStatus(string userId, string articleCode, Enums.ArticleStatus articleStatus)
        {
            TableName = "UserArticleStatus";
            CustomerId = userId;
            ArticleCode = articleCode;
            ArticleStatus = articleStatus;
        }

        protected override bool IsEqualImp(ISyncItem otherItem)
        {
            UserArticleStatus castedOtherItem = (UserArticleStatus)otherItem;
            return (ArticleCode == castedOtherItem.ArticleCode) && (ArticleStatus == castedOtherItem.ArticleStatus);
        }

        protected override async Task UpdateImp()
        {
            UserArticleStatus queriedArticleStatus = await LocalDb.Instance.GetUserArticleStatus(ArticleCode);
            queriedArticleStatus.ArticleStatus = ArticleStatus;

            await LocalDb.Instance.UpdateSyncItem(this);
        }

        protected override async Task<ISyncItem> GetLocalItemCopyImp()
        {
            return await LocalDb.Instance.GetUserArticleStatus(ArticleCode);
        }
    }
}
