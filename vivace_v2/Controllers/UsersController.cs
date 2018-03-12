using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vivace.Models;

namespace vivace.Controllers
{

    [Route("api/users")]
    public class UsersController : ControllerVivace<User>
    {

        protected override string COLLECTION_NAME { get { return "Users"; } }

        protected override string ITEM_NOT_FOUND { get { return "User ID not found"; } }

        public UsersController(ICosmosRepository cr) : base(cr)
        { }

        // PUT api/<controller>/5/addband/5
        [HttpPut("{userid}/addband/{bandid}")]
        public async Task<IActionResult> AddBand(string userid, string bandid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // get user from DB
            User user = await GetDocFromDB(userid);

            if (user == null)
            {
                return NotFound(ITEM_NOT_FOUND);
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
            User newUser = await ReplaceDocInDB(userid, user);

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
            User user = await GetDocFromDB(userid);

            if (user == null)
            {
                return NotFound(ITEM_NOT_FOUND);
            }

            // remove band from list
            List<string> bands = user.Bands.ToList();
            bands.Remove(bandid);
            user.Bands = bands;

            // update database
            User newUser = await ReplaceDocInDB(userid, user);
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
            User user = await GetDocFromDB(userid);

            if (user == null)
            {
                return NotFound(ITEM_NOT_FOUND);
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
            User newUser = await ReplaceDocInDB(userid, user);

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
            User user = await GetDocFromDB(userid);

            if (user == null)
            {
                return NotFound(ITEM_NOT_FOUND);
            }

            // remove event from list
            List<string> events = user.Events.ToList();
            events.Remove(eventid);
            user.Events = events;

            // update database
            User newUser = await ReplaceDocInDB(userid, user);
            return Ok(newUser);
        }
    }
}
