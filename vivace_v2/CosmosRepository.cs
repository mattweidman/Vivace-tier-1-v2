using System;
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

        /// <summary>
        /// Gets a document from a collection in the Vivace database.
        /// Returns null if not found.
        /// </summary>
        /// <param name="collectionName">Name of collection</param>
        /// <param name="id">ID of value looking for</param>
        /// <returns></returns>
        public async Task<Document> GetFromDB(string collectionName, string id)
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
    }
}
