using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using vivace.Models;

namespace vivace.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ControllerVivace<User>
    {

        public override string COLLECTION_NAME { get { return "Users"; } }

        protected string USERNAME_NOT_FOUND { get { return NotFoundMessage("Username"); } }

        protected string INVALID_USERNAME { get {
                return "Username may only contain alphanumeric characters and underscores."; } }

        protected string USERNAME_EXISTS { get { return "Username already exists."; } }

        public UsersController(ICosmosRepository cr) : base(cr)
        { }

        private string NotFoundMessage(string prop)
        {
            return prop + " not found";
        }

        /// <summary>
        /// Makes sure username only contains alphanumeric characters or underscores.
        /// </summary>
        /// <param name="username">possible username</param>
        /// <returns>true if username is invalid</returns>
        protected bool IsInvalidUsername(string username)
        {
            Regex r = new Regex("[^a-zA-Z0-9_]");
            return r.Match(username).Success;
        }

        // POST api/<controller>
        [HttpPost]
        public override async Task<IActionResult> Post([FromBody]User docIn)
        {
            string username = docIn.Username;

            // make sure username is included
            if (username == null)
            {
                return MissingPropertyResult("username");
            }

            // make sure username is valid
            if (IsInvalidUsername(username))
            {
                return BadRequest(INVALID_USERNAME);
            }

            // make sure username does not already exist
            Microsoft.Azure.Documents.Document doc = await CosmosRepo.QueryDocument(COLLECTION_NAME,
                $"SELECT * FROM Users u WHERE u.username='{username}'");
            if (doc != null)
            {
                return BadRequest(USERNAME_EXISTS);
            }

            // set list fields to empty lists
            docIn.Bands = new List<string>();
            docIn.Events = new List<string>();

            return await base.Post(docIn);
        }

        // GET api/<controller>/username/<username>
        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetUserByName(string username)
        {
            // username must be all alphanumeric characters or underscores
            if (IsInvalidUsername(username))
            {
                return BadRequest(INVALID_USERNAME);
            }

            // SQL query
            Microsoft.Azure.Documents.Document doc = await CosmosRepo.QueryDocument(COLLECTION_NAME, 
                $"SELECT * FROM Users u WHERE u.username='{username}'");

            if (doc == null)
            {
                return NotFound(USERNAME_NOT_FOUND);
            }

            return Ok((User)(dynamic)doc);
        }

        // PUT api/<controller>/5/addband/5
        [HttpPut("{userid}/addband/{bandid}")]
        public async Task<IActionResult> AddBand(string userid, string bandid)
        {
            return await ChangeTwoDBs<Band>(userid, bandid, (new BandsController(CosmosRepo)).COLLECTION_NAME,
                user =>
                {
                    if (!user.Bands.Contains(bandid))
                    {
                        List<string> bands = user.Bands.ToList();
                        bands.Add(bandid);
                        user.Bands = bands;
                    }
                    return user;
                }, 
                band =>
                {
                    if (!band.Users.Contains(userid))
                    {
                        List<string> bandUsers = band.Users.ToList();
                        bandUsers.Add(userid);
                        band.Users = bandUsers;
                    }
                    return band;
                }
            );
        }

        // PUT api/<controller>/5/leaveband/5
        [HttpPut("{userid}/leaveband/{bandid}")]
        public async Task<IActionResult> LeaveBand(string userid, string bandid)
        {
            return await ChangeTwoDBs<Band>(userid, bandid, (new BandsController(CosmosRepo)).COLLECTION_NAME,
                user =>
                {
                    List<string> bands = user.Bands.ToList();
                    bands.Remove(bandid);
                    user.Bands = bands;
                    return user;
                },
                band =>
                {
                    List<string> users = band.Users.ToList();
                    users.Remove(userid);
                    band.Users = users;
                    return band;
                }
            );
        }

        // PUT api/<controller>/5/addevent/5
        [HttpPut("{userid}/addevent/{eventid}")]
        public async Task<IActionResult> AddEvent(string userid, string eventid)
        {
            return await ChangeTwoDBs<Event>(userid, eventid, (new EventsController(CosmosRepo)).COLLECTION_NAME,
                user =>
                {
                    if (!user.Events.Contains(eventid))
                    {
                        List<string> events = user.Events.ToList();
                        events.Add(eventid);
                        user.Events = events;
                    }
                    return user;
                },
                event_ =>
                {
                    if (!event_.Users.Contains(userid))
                    {
                        List<string> users = event_.Users.ToList();
                        users.Add(userid);
                        event_.Users = users;
                    }
                    return event_;
                }
            );
        }

        // PUT api/<controller>/5/leaveevent/5
        [HttpPut("{userid}/leaveevent/{eventid}")]
        public async Task<IActionResult> LeaveEvent(string userid, string eventid)
        {
            return await ChangeTwoDBs<Event>(userid, eventid, (new EventsController(CosmosRepo)).COLLECTION_NAME,
                user =>
                {
                    List<string> events = user.Events.ToList();
                    events.Remove(eventid);
                    user.Events = events;
                    return user;
                },
                event_ =>
                {
                    List<string> users = event_.Users.ToList();
                    users.Remove(userid);
                    event_.Users = users;
                    return event_;
                }
            );
        }
    }
}
