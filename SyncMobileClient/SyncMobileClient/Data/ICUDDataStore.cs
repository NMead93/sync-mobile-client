﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncMobileClient.Models;

namespace SyncMobileClient.Data
{
    public interface ICUDDataStore
    {
        Task<IEnumerable<ICUDEntity>> GetCUD();

        void AddCUDEntity(string tableName, string recordId, string customerId, string payload, string operation, DateTimeOffset operationDate);

        void CleanUp(DateTimeOffset updatedTimeSync);
    }
}
