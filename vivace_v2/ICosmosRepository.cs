﻿using Microsoft.Azure.Documents;
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
        Task<Document> GetDocument(string collectionName, string id);

        /// <summary>
        /// Create a new document in the database.
        /// </summary>
        /// <param name="collectionName">Name of collection</param>
        /// <param name="obj">C# object to store as a JSON document</param>
        /// <returns></returns>
        Task<Document> CreateDocument(string collectionName, object obj);
    }
}
