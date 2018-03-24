using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

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
        
        public async Task<Document> GetDocument(string collectionName, string id)
        {
            try
            {
                Document result = await client.ReadDocumentAsync(
                    UriFactory.CreateDocumentUri(DBName, collectionName, id));

                return result;
            }
            catch (DocumentClientException)
            {
                return null;
            }
        }

        public async Task<Document> QueryDocument(string collectionName, string sqlQuerySpec)
        {
            return await Task.Run(() =>
            {
                IQueryable<dynamic> results = client.CreateDocumentQuery(
                    UriFactory.CreateDocumentCollectionUri(DBName, collectionName), sqlQuerySpec);
                foreach (dynamic d in results)
                {
                    return d;
                }
                return null;
            });
        }

        public async Task<Document> CreateDocument(string collectionName, object obj)
        {
            return await client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(DBName, collectionName), obj);
        }

        public async Task<Document> ReplaceDocument(string collectionName, string id, object obj)
        {
            return await client.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri(DBName, collectionName, id), obj);
        }
    }
}
