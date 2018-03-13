using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vivace.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace vivace.Controllers
{
    [Route("api/[controller]")]
    public class SongsController : ControllerVivace<Song>
    {
        protected override string COLLECTION_NAME { get { return "Songs"; } }

        protected override string ITEM_NOT_FOUND { get { return "Song ID not found"; } }

        public SongsController(ICosmosRepository cr) : base(cr)
        { }

        // PUT api/<controller>/5/addpart/5
        [HttpPut("{songid}/addpart/{partid}")]
        public async Task<IActionResult> AddPart(string songid, string partid)
        {
            return await ChangeInDB(songid, song =>
            {
                List<string> parts = song.Parts.ToList();

                if (!parts.Contains(partid))
                {
                    parts.Add(partid);
                    song.Parts = parts;
                }

                return song;
            });
        }

        // PUT api/<controller>/5/deletepart/5
        [HttpPut("{songid}/deletepart/{partid}")]
        public async Task<IActionResult> DeletePart(string songid, string partid)
        {
            return await ChangeInDB(songid, song =>
            {
                List<string> parts = song.Parts.ToList();
                parts.Remove(partid);
                song.Parts = parts;
                return song;
            });
        }

        // PUT api/<controller>/5/rename
        [HttpPut("{songid}/rename")]
        public async Task<IActionResult> Rename(string songid, [FromBody]Song replacement)
        {
            // The user is supposed to put something like
            // { "name": "songname" } in the body.
            // If the user gives any other properties besides name,
            // they will be ignored. 

            return await ChangeInDB(songid, song =>
            {
                song.Name = replacement.Name;
                return song;
            });
        }
    }
}
