using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vivace.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace vivace.Controllers
{

    [Route("api/Users")]
    public class UsersController : Controller
    {

        private static readonly string COLLECTION_NAME = "Users";
        private static readonly string USER_NOT_FOUND = "User ID not found";

        private ICosmosRepository CosmosRepo;

        public UsersController(ICosmosRepository cr)
        {
            CosmosRepo = cr;
        }

        /// <summary>
        /// Get hostname URL
        /// </summary>
        /// <returns></returns>
        private string GetHost()
        {
            return Request.Host.ToString();
        }

        /// <summary>
        /// Gets the URI of a get request for some id
        /// </summary>
        /// <returns></returns>
        private string GetGetUri(string id)
        {
            return GetHost() + $"/api/Users/{id}";
        }

        /// <summary>
        /// Calls Cosmos to get a user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns></returns>
        private async Task<User> GetUserFromDB(string id)
        {
            Microsoft.Azure.Documents.Document doc = 
                await CosmosRepo.GetDocument(COLLECTION_NAME, id);
            return (User)(dynamic)doc;
        }

        /// <summary>
        /// Call Cosmos to replace a user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="user">object to replace with</param>
        /// <returns></returns>
        private async Task<User> ReplaceUserInDB(string id, User user)
        {
            Microsoft.Azure.Documents.Document doc =
                await CosmosRepo.ReplaceDocument(COLLECTION_NAME, id, user);
            return (User)(dynamic)doc;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            User user = await GetUserFromDB(id);

            if (user == null)
            {
                return NotFound(USER_NOT_FOUND);
            }

            return Ok(user);
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Microsoft.Azure.Documents.Document doc = await CosmosRepo.CreateDocument(COLLECTION_NAME, user);
            User newUser = (User)(dynamic)doc;

            return Created(GetGetUri(newUser.Id), newUser);
        }

        // PUT api/<controller>/5/addband/5
        [HttpPut("{userid}/addband/{bandid}")]
        public async Task<IActionResult> AddBand(string userid, string bandid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // get user from DB
            User user = await GetUserFromDB(userid);

            if (user == null)
            {
                return NotFound(USER_NOT_FOUND);
            }

            // check if band already exists in user's list
            List<string> bands = user.Bands.ToList();
            if (bands.Contains(bandid))
            {
                return Ok(user);
            }

            // add bandid to user
            bands.Add(bandid);
            user.Bands = bands;

            // update database
            User newUser = await ReplaceUserInDB(userid, user);

            // return user
            return Ok(newUser);
        }

        // PUT api/<controller>/5/leaveband/5
        [HttpPut("{userid}/leaveband/{bandid}")]
        public async Task<IActionResult> LeaveBand(string userid, string bandid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // get user from DB
            User user = await GetUserFromDB(userid);

            if (user == null)
            {
                return NotFound(USER_NOT_FOUND);
            }

            // remove band from list
            List<string> bands = user.Bands.ToList();
            bands.Remove(bandid);
            user.Bands = bands;

            // update database
            User newUser = await ReplaceUserInDB(userid, user);
            return Ok(newUser);
        }

        // PUT api/<controller>/5/addevent/5
        [HttpPut("{userid}/addevent/{eventid}")]
        public async Task<IActionResult> AddEvent(string userid, string eventid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // get user from DB
            User user = await GetUserFromDB(userid);

            if (user == null)
            {
                return NotFound(USER_NOT_FOUND);
            }

            // check if event already exists in user's list
            List<string> events = user.Events.ToList();
            if (events.Contains(eventid))
            {
                return Ok(user);
            }

            // add eventid to user
            events.Add(eventid);
            user.Events = events;

            // update database
            User newUser = await ReplaceUserInDB(userid, user);

            // return user
            return Ok(newUser);
        }

        // PUT api/<controller>/5/leaveevent/5
        [HttpPut("{userid}/leaveevent/{eventid}")]
        public async Task<IActionResult> LeaveEvent(string userid, string eventid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // get user from DB
            User user = await GetUserFromDB(userid);

            if (user == null)
            {
                return NotFound(USER_NOT_FOUND);
            }

            // remove event from list
            List<string> events = user.Events.ToList();
            events.Remove(eventid);
            user.Events = events;

            // update database
            User newUser = await ReplaceUserInDB(userid, user);
            return Ok(newUser);
        }
    }
}
