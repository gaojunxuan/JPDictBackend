using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Text;
using JPDictBackend.Model;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace JPDictBackend.Helper
{
    public static class AzureStorageHelper
    {        
        public static async Task<DailySentenceResult> RetrieveData(string dateString,CloudTable table)
        {
            string yearstr = dateString.Substring(0, 4);
            TableOperation retrieveOperation = TableOperation.Retrieve<DailySentenceResult>(yearstr,dateString);
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                return retrievedResult.Result as DailySentenceResult;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}
