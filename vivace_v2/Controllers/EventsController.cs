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
        public override string CollectionName { get { return CollectionNames.EVENTS; } }

        public EventsController(ICosmosRepository cr) : base(cr)
        { }

        /// <summary>
        /// Message returned when song not found in band.
        /// </summary>
        /// <param name="songId">song ID</param>
        /// <returns></returns>
        protected string SongNotInBandMessage(string songId)
        {
            return "Song " + songId + " not found in band";
        }

        /// <summary>
        /// Sends an HTTP request to get the band for this event
        /// </summary>
        /// <param name="bandId">Band ID</param>
        /// <returns></returns>
        protected async Task<Band> GetBand(string bandId)
        {
            string bandCollection = CollectionNames.BANDS;
            return await CosmosRepo.GetDocument<Band>(bandCollection, bandId);
        }

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
            docIn.Songs = new List<string>();
            docIn.Users = new List<string>();
            docIn.CurrentSong = null;

            // update band
            if (!band.Events.Contains(eventId))
            {
                List<string> events = band.Events.ToList();
                events.Add(eventId);
                band.Events = events;
                await CosmosRepo.ReplaceDocument(bandCollection, bandId, band);
            }

            return Created(GetGetUri(eventId), newEvent);
        }

        // PUT api/<controller>/5/addsong/5
        [HttpPut("{eventid}/addsong/{songid}")]
        public async Task<IActionResult> AddSong(string eventid, string songid)
        {
            return await CheckAndChangeInDB<Band>(eventid, 
                event_ => event_.Band,
                CollectionNames.BANDS,
                band => band.Songs.Contains(songid),
                event_ =>
                {
                    List<string> songs = event_.Songs.ToList();
                    if (!songs.Contains(songid))
                    {
                        songs.Add(songid);
                        event_.Songs = songs;
                    }
                    return event_;
                },
                NotFound(SongNotInBandMessage(songid))
            );
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

            string songId = replacement.CurrentSong;
            return await CheckAndChangeInDB<Band>(eventid,
                event_ => event_.Band,
                CollectionNames.BANDS,
                band => band.Songs.Contains(songId),
                event_ =>
                {
                    event_.CurrentSong = songId;
                    return event_;
                },
                NotFound(SongNotInBandMessage(songId))
            );
        }

    }
}
