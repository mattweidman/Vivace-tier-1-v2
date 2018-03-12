using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using vivace.Models;

namespace vivace.Controllers
{
    /// <summary>
    /// Superclass of controllers used in Vivace
    /// </summary>
    /// <typeparam name="T">Model type</typeparam>
    public abstract class ControllerVivace<T> : ControllerBase where T : ModelVivace
    {
        /// <summary>
        /// Name of this collection
        /// </summary>
        protected abstract string COLLECTION_NAME { get; }

        /// <summary>
        /// What to output in a 404 when ID of document not found
        /// </summary>
        protected abstract string ITEM_NOT_FOUND { get; }

        /// <summary>
        /// Collection of methods used to interract with Cosmos;
        /// used for dependency injection.
        /// </summary>
        protected ICosmosRepository CosmosRepo;

        /// <summary>
        /// Create a new Vivace controller
        /// </summary>
        /// <param name="cr">Cosmos repository</param>
        public ControllerVivace(ICosmosRepository cr)
        {
            CosmosRepo = cr;
        }

        /// <summary>
        /// Get hostname URL
        /// </summary>
        /// <returns></returns>
        protected string GetHost()
        {
            return Request.Host.ToString();
        }

        /// <summary>
        /// Gets the URI of a get request for some id
        /// </summary>
        /// <returns></returns>
        protected string GetGetUri(string id)
        {
            return GetHost() + $"/api/{COLLECTION_NAME}/{id}";
        }

        /// <summary>
        /// Calls Cosmos to get a document by ID
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns></returns>
        protected async Task<T> GetDocFromDB(string id)
        {
            Microsoft.Azure.Documents.Document doc =
                await CosmosRepo.GetDocument(COLLECTION_NAME, id);
            return (T)(dynamic)doc;
        }

        /// <summary>
        /// Call Cosmos to replace a document by ID
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <param name="replace">object to replace with</param>
        /// <returns></returns>
        protected async Task<T> ReplaceDocInDB(string id, T replace)
        {
            Microsoft.Azure.Documents.Document doc =
                await CosmosRepo.ReplaceDocument(COLLECTION_NAME, id, replace);
            return (T)(dynamic)doc;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            T doc = await GetDocFromDB(id);

            if (doc == null)
            {
                return NotFound(ITEM_NOT_FOUND);
            }

            return Ok(doc);
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]T docIn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Microsoft.Azure.Documents.Document doc = await CosmosRepo.CreateDocument(COLLECTION_NAME, docIn);
            T newDoc = (T)(dynamic)doc;

            return Created(GetGetUri(newDoc.Id), newDoc);
        }

        /// <summary>
        /// Gets a document by ID, changes it in some way, 
        /// and replaces it in the database.
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <param name="op">Function that converts document into new 
        /// document before replacing in database</param>
        /// <returns>HTTP result: 500 if error, 404 if ID not found, 
        /// else 200 with updated document</returns>
        protected async Task<IActionResult> ChangeInDB(string id, Func<T, T> op)
        {
            // check model state first
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // get user from DB
            T docGot = await GetDocFromDB(id);

            // return 404 if not found
            if (docGot == null)
            {
                return NotFound(ITEM_NOT_FOUND);
            }

            // use function
            T converted = op(docGot);

            // update database
            T result = await ReplaceDocInDB(id, converted);
            return Ok(result);
        }
    }
}
