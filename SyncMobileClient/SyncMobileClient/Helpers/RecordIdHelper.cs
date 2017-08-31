using SyncMobileClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncMobileClient.Helpers
{
    public class RecordIdHelper
    {
        public static string CreateRecordId(ISyncItem item)
        {
            string uniqueTableColumnId = "";

            if (item.TableName == "UserPreference")
            {
                uniqueTableColumnId = "PreferenceName";
            }
            else if (item.TableName == "UserArticleStatus")
            {
                uniqueTableColumnId = "ArticleCode";
            }
            else
            {
                //continue on
            }

            return item.CustomerId + "_" + uniqueTableColumnId;
        }
    }
}
