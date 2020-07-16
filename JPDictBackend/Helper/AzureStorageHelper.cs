using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Text;
using JPDictBackend.Model;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;

namespace JPDictBackend.Helper
{
    public static class AzureStorageHelper
    {        
        public static async Task<DailySentenceResult> RetrieveEverydaySentenceData(string dateString, CloudTable table)
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
        public static async Task<DailySentenceResult> RetrieveEverydaySentenceData(int index, CloudTable table)
        {
            var query = new TableQuery<DailySentenceResult>().Where(TableQuery.GenerateFilterCondition("Index", QueryComparisons.Equal, index.ToString()));
            TableContinuationToken token = null;
            var entities = new List<DailySentenceResult>();
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            }
            while (token != null);
            return entities.FirstOrDefault();
        }
        public static async Task<List<DictUpdate>> RetrieveUpdateData(CloudTable table)
        {
            TableContinuationToken token = null;
            var entities = new List<DictUpdate>();
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(new TableQuery<DictUpdate>(), token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            }
            while (token != null);
            return entities;
        }
        public static async Task<List<WebDict>> QueryWebDict(CloudTable table, string keyword)
        {
            var query = new TableQuery<WebDict>().Where(TableQuery.GenerateFilterCondition("Keyword",QueryComparisons.Equal,keyword));
            TableContinuationToken token = null;
            var entities = new List<WebDict>();
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            }
            while (token != null);
            return entities;
        }
        public static async Task<List<WebDict>> FuzzyQueryWebDict(CloudTable table, string keyword)
        {
            var query = new TableQuery<WebDict>().Where(TableQuery.GenerateFilterCondition("Keyword", QueryComparisons.GreaterThanOrEqual, keyword));
            TableContinuationToken token = null;
            var entities = new List<WebDict>();
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            }
            while (token != null);
            return entities;
        }
    }
}
