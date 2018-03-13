using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vivace.Models;

namespace vivace.Controllers
{
    [Route("api/[controller]")]
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
            return await ChangeInDB(userid, user =>
            {
                List<string> bands = user.Bands.ToList();

                // check if band already exists in user's list
                if (!bands.Contains(bandid))
                {
                    // add bandid to user
                    bands.Add(bandid);
                    user.Bands = bands;
                }

                return user;
            });
        }

        // PUT api/<controller>/5/leaveband/5
        [HttpPut("{userid}/leaveband/{bandid}")]
        public async Task<IActionResult> LeaveBand(string userid, string bandid)
        {
            return await ChangeInDB(userid, user =>
            {
                // remove band from list if it exists
                List<string> bands = user.Bands.ToList();
                bands.Remove(bandid);
                user.Bands = bands;
                return user;
            });
        }

        // PUT api/<controller>/5/addevent/5
        [HttpPut("{userid}/addevent/{eventid}")]
        public async Task<IActionResult> AddEvent(string userid, string eventid)
        {
            return await ChangeInDB(userid, user =>
            {
                List<string> events = user.Events.ToList();

                // check if event already exists in user's list
                if (!events.Contains(eventid))
                {
                    // add eventid to user
                    events.Add(eventid);
                    user.Events = events;
                }

                return user;
            });
        }

        // PUT api/<controller>/5/leaveevent/5
        [HttpPut("{userid}/leaveevent/{eventid}")]
        public async Task<IActionResult> LeaveEvent(string userid, string eventid)
        {
            return await ChangeInDB(userid, user =>
            {
                // remove event from list if it exists
                List<string> events = user.Events.ToList();
                events.Remove(eventid);
                user.Events = events;
                return user;
            });
        }
    }
}
