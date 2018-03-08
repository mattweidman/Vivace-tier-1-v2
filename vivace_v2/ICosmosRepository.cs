using Microsoft.Azure.Documents;
using System.Threading.Tasks;

namespace vivace
{
    public interface ICosmosRepository
    {
        /// <summary>
        /// Gets a document from a collection in a database.
        /// Returns null if not found.
        /// </summary>
        /// <param name="collectionName">Name of collection</param>
        /// <param name="id">ID of value looking for</param>
        /// <returns></returns>
        Task<Document> GetFromDB(string collectionName, string id);
    }
}
