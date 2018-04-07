using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace vivace
{
    public class CosmosRepository : ICosmosRepository
    {
        private const string EndpointUrl = "https://vivace.documents.azure.com:443/";
        private const string PrimaryKey = "1PpmE7ZwUDg03JQN17KGaQyxInfsHKSaurgV4vaIDgXRFpH1QOXaVZaTJhvImilMSSE3jMiuWB95PDjuxw4yTA==";
        private const string DBName = "Vivace";

        private DocumentClient client;

        public CosmosRepository ()
        {
            client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
        }
        
        public async Task<T> GetDocument<T>(string collectionName, string id)
        {
            Document result = await client.ReadDocumentAsync(
                UriFactory.CreateDocumentUri(DBName, collectionName, id));

            return (dynamic)result;
        }

        public async Task<T> QueryDocument<T>(string collectionName, string sqlQuerySpec)
        {
            return await Task.Run(() =>
            {
                IQueryable<dynamic> results = client.CreateDocumentQuery(
                    UriFactory.CreateDocumentCollectionUri(DBName, collectionName), sqlQuerySpec);
                foreach (dynamic d in results)
                {
                    return d;
                }
                throw new DocumentQueryException("SQL query found nothing in " + collectionName);
            });
        }

        public async Task<T> CreateDocument<T>(string collectionName, T obj)
        {
            return (dynamic)(await client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(DBName, collectionName), obj));
        }

        public async Task<T> ReplaceDocument<T>(string collectionName, string id, T obj)
        {
            return (dynamic)(await client.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri(DBName, collectionName, id), obj));
        }
    }
}
