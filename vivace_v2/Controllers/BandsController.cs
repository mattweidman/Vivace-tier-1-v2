using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vivace.Models;

namespace vivace.Controllers
{
    [Route("api/bands")]
    public class BandsController : ControllerVivace<Band>
    {
        protected override string COLLECTION_NAME { get { return "Bands"; } }

        protected override string ITEM_NOT_FOUND { get { return "Band ID not found"; } }

        public BandsController(ICosmosRepository cr) : base(cr)
        { }

        // PUT api/<controller>/5/addsong/5
        [HttpPut("{bandid}/addsong/{songid}")]
        public async Task<IActionResult> AddSong(string bandid, string songid)
        {
            return await ChangeInDB(bandid, band =>
            {
                List<string> songs = band.Songs.ToList();

                if (!songs.Contains(songid))
                {
                    songs.Add(songid);
                    band.Songs = songs;
                }

                return band;
            });
        }

        // PUT api/<controller>/5/deletesong/5
        [HttpPut("{bandid}/deletesong/{songid}")]
        public async Task<IActionResult> DeleteSong(string bandid, string songid)
        {
            return await ChangeInDB(bandid, band =>
            {
                List<string> songs = band.Songs.ToList();
                songs.Remove(songid);
                band.Songs = songs;
                return band;
            });
        }

        // PUT api/<controller>/5/rename
        [HttpPut("{bandid}/rename")]
        public async Task<IActionResult> Rename(string bandid, [FromBody]Band replacement)
        {
            // The user is supposed to put something like
            // { "name": "bandname" } in the body.
            // If the user gives any other properties besides name,
            // they will be ignored. 

            return await ChangeInDB(bandid, band =>
            {
                band.Name = replacement.Name;
                return band;
            });
        }
    }
}
