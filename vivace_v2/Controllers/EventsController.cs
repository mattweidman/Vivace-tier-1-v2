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
        public override string COLLECTION_NAME { get { return "Events"; } }

        protected override string ITEM_NOT_FOUND { get { return "Event ID not found"; } }

        public EventsController(ICosmosRepository cr) : base(cr)
        { }

        // POST api/<controller>
        [HttpPost]
        public override async Task<IActionResult> Post([FromBody]Event docIn)
        {
            // check required properties
            if (docIn.Name == null)
            {
                return MissingPropertyResult("name");
            }
            if (docIn.Band == null)
            {
                return MissingPropertyResult("band");
            }

            // remove unused properties
            if (docIn.Songs == null)
            {
                docIn.Songs = new List<string>();
            }
            docIn.Users = new List<string>();
            docIn.CurrentSong = null;

            return await base.Post(docIn);
        }

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
            if (replacement.Name == null)
            {
                return MissingPropertyResult("name");
            }

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
            if (replacement.CurrentSong == null)
            {
                return MissingPropertyResult("currentsong");
            }

            return await ChangeInDB(eventid, event_ =>
            {
                event_.CurrentSong = replacement.CurrentSong;
                return event_;
            });
        }

    }
}
