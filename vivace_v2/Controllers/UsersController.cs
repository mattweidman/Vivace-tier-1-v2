using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vivace.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace vivace.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        // counts how many users have been created
        private static int IdCounter = 0;

        private static readonly string DEFAULT_USERNAME = "username";
        private static readonly string USER_NOT_FOUND = "User ID not found";
        private static readonly string BAND_NOT_FOUND = "User not subscribed to band ID";
        private static readonly string EVENT_NOT_FOUND = "User not subscribed to event ID";

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (id >= 0 && id < IdCounter)
            {
                return Ok(new User
                {
                    Id = id.ToString(),
                    Username = DEFAULT_USERNAME,
                    Bands = new string[] { "1" },
                    Events = new string[] { "2" }
                });
            }

            return NotFound(USER_NOT_FOUND);
        }

        // POST api/<controller>
        [HttpPost]
        public IActionResult Post([FromBody]User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // save to database
            user.Id = IdCounter.ToString();
            IdCounter++;
            
            return Ok(user);
        }

        // PUT api/<controller>/5/addband/5
        [HttpPut("{userid}/addband/{bandid}")]
        public IActionResult AddBand(int userid, int bandid)
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

            // add band to user entry in database

            return Ok(new User
            {
                Id = userid.ToString(),
                Username = DEFAULT_USERNAME,
                Bands = new string[] { "1", bandid.ToString() },
                Events = new string[] { "2" }
            });
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
