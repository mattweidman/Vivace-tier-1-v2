using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vivace.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace vivace.Controllers
{
    [Route("api/[controller]")]
    public class EventsController : ControllerVivace<Event>
    {
        protected override string COLLECTION_NAME { get { return "Events"; } }

        protected override string ITEM_NOT_FOUND { get { return "Event ID not found"; } }

        public EventsController(ICosmosRepository cr) : base(cr)
        { }

        // PUT api/<controller>/5/addsong/5
        [HttpPut("{eventid}/addsong/{songid}")]
        public async Task<IActionResult> AddSong(string eventid, string songid)
        {
            return await ChangeInDB(eventid, event_ =>
            {
                List<string> songs = event_.Songs.ToList();

                if (!songs.Contains(songid))
                {
                    songs.Add(songid);
                    event_.Songs = songs;
                }

                return event_;
            });
        }

        // PUT api/<controller>/5/deletesong/5
        [HttpPut("{eventid}/deletesong/{songid}")]
        public async Task<IActionResult> DeleteSong(string eventid, string songid)
        {
            return await ChangeInDB(eventid, event_ =>
            {
                List<string> songs = event_.Songs.ToList();
                songs.Remove(songid);
                event_.Songs = songs;
                return event_;
            });
        }

        // PUT api/<controller>/5/rename
        [HttpPut("{eventid}/rename")]
        public async Task<IActionResult> Rename(string eventid, [FromBody]Event replacement)
        {
            // The user is supposed to put something like
            // { "name": "eventname" } in the body.
            // If the user gives any other properties besides name,
            // they will be ignored. 

            return await ChangeInDB(eventid, event_ =>
            {
                event_.Name = replacement.Name;
                return event_;
            });
        }

        // PUT api/<controller>/5/changecurrentsong
        [HttpPut("{eventid}/changecurrentsong")]
        public async Task<IActionResult> ChangeCurrentSong(string eventid, [FromBody]Event replacement)
        {
            return await ChangeInDB(eventid, event_ =>
            {
                event_.CurrentSong = replacement.CurrentSong;
                return event_;
            });
        }

    }
}
