using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vivace.Models;

namespace vivace
{
    /// <summary>
    /// Mock database that just holds documents in hash tables
    /// </summary>
    public class MockCosmosRepository : ICosmosRepository
    {
        Dictionary<string, Dictionary<string, object>> mockDatabase;

        public MockCosmosRepository()
        {
            mockDatabase = new Dictionary<string, Dictionary<string, object>>
            {
                { CollectionNames.PLAYERS, new Dictionary<string, object>() },
                { CollectionNames.BANDS, new Dictionary<string, object>() },
                { CollectionNames.EVENTS, new Dictionary<string, object>() },
                { CollectionNames.SONGS, new Dictionary<string, object>() },
                { CollectionNames.PARTS, new Dictionary<string, object>() },
            };
        }

        public Task<T> CreateDocument<T>(string collectionName, T obj)
        {
            if (typeof(T).IsSubclassOf(typeof(ModelVivace)))
            {
                ModelVivace mv = obj as ModelVivace;
                string id = Guid.NewGuid().ToString();
                mv.Id = id;
                mockDatabase[collectionName].Add(id, mv);
                return Task.FromResult((T)(object)mv);
            }

            throw new DocumentQueryException("Can only create documents of type ModelVivace");
        }

        public Task<T> GetDocument<T>(string collectionName, string id)
        {
            Dictionary<string, object> collection = mockDatabase[collectionName];
            return Task.FromResult((T)collection[id]);
        }

        /// <summary>
        /// Only works for querying players. Assumes sqlQuerySpec starts with 
        /// "SELECT * FROM Players u WHERE u.username="
        /// </summary>
        public Task<T> QueryDocument<T>(string collectionName, string sqlQuerySpec)
        {
            Dictionary<string, object> collection = mockDatabase[collectionName];
            int eqIndex = sqlQuerySpec.LastIndexOf("=");
            string targetVal = sqlQuerySpec.Substring(eqIndex + 1);

            if (typeof(T).IsSubclassOf(typeof(ModelVivace)))
            {
                foreach (object doc in collection.Values)
                {
                    if (doc is Player p && p.Username.Equals(targetVal))
                    {
                        return Task.FromResult((T)(object)p);
                    }
                }

                throw new DocumentQueryException("Target " + targetVal + " not found in " + collectionName);
            }

            throw new DocumentQueryException("Can only query documents of type Player");
        }

        public Task<T> ReplaceDocument<T>(string collectionName, string id, T obj)
        {
            mockDatabase[collectionName][id] = obj;
            return Task.FromResult((T)mockDatabase[collectionName][id]);
        }
    }
}
