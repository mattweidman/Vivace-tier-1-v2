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
        // counts how many users have been created
        private static int IdCounter = 0;

        private static readonly string COLLECTION_NAME = "Users";
        private static readonly string DEFAULT_USERNAME = "username";
        private static readonly string USER_NOT_FOUND = "User ID not found";
        private static readonly string BAND_NOT_FOUND = "User not subscribed to band ID";
        private static readonly string EVENT_NOT_FOUND = "User not subscribed to event ID";

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

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            Microsoft.Azure.Documents.Document doc = await CosmosRepo.GetDocument(COLLECTION_NAME, id);
            User user = (User)(dynamic)doc;

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
            Microsoft.Azure.Documents.Document doc = await CosmosRepo.GetDocument(COLLECTION_NAME, userid);
            User user = (User)(dynamic)doc;

            if (user == null)
            {
                return NotFound(USER_NOT_FOUND);
            }

            // check if band already exists in user's list
            foreach (string otherBand in user.Bands)
            {
                if (otherBand.Equals(bandid))
                {
                    return Ok(user);
                }
            }

            // add bandid to user
            List<string> bands = user.Bands.ToList();
            bands.Add(bandid);
            user.Bands = bands;

            // update database
            doc = await CosmosRepo.ReplaceDocument(COLLECTION_NAME, userid, user);
            User newUser = (User)(dynamic)doc;

            // return user
            return Ok(newUser);
        }

        // PUT api/<controller>/5/leaveband/5
        [HttpPut("{userid}/leaveband/{bandid}")]
        public IActionResult LeaveBand(int userid, int bandid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // check database if user exists
            if (userid < 0 || userid >= IdCounter)
            {
                return NotFound(USER_NOT_FOUND);
            }

            // remove band from user entry in database

            // if band was found and removed, OK
            if (bandid == 1)
            {
                return Ok(new User
                {
                    Id = userid.ToString(),
                    Username = DEFAULT_USERNAME,
                    Bands = new string[] { },
                    Events = new string[] { "2" }
                });
            }

            // otherwise not found
            else
            {
                return NotFound(BAND_NOT_FOUND);
            }
        }

        // PUT api/<controller>/5/addevent/5
        [HttpPut("{userid}/addevent/{eventid}")]
        public IActionResult AddEvent(int userid, int eventid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // check database if user exists
            if (userid < 0 || userid >= IdCounter)
            {
                return NotFound(USER_NOT_FOUND);
            }

            // add event to user entry in database

            return Ok(new User
            {
                Id = userid.ToString(),
                Username = DEFAULT_USERNAME,
                Bands = new string[] { "1" },
                Events = new string[] { "2", eventid.ToString() }
            });
        }

        // PUT api/<controller>/5/leaveevent/5
        [HttpPut("{userid}/leaveevent/{eventid}")]
        public IActionResult LeaveEvent(int userid, int eventid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // check database if user exists
            if (userid < 0 || userid >= IdCounter)
            {
                return NotFound(USER_NOT_FOUND);
            }

            // remove event from user entry in database

            // if event was found and removed, OK
            if (eventid == 2)
            {
                return Ok(new User
                {
                    Id = userid.ToString(),
                    Username = DEFAULT_USERNAME,
                    Bands = new string[] { "1" },
                    Events = new string[] { }
                });
            }

            // otherwise not found
            else
            {
                return NotFound(EVENT_NOT_FOUND);
            }
        }
    }
}
