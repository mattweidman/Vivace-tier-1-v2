using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using vivace.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace vivace.Controllers
{
    [Route("api/[controller]")]
    public class PartsController : ControllerVivace<Part>
    {
        public override string CollectionName { get { return CollectionNames.PARTS; } }

        public PartsController(ICosmosRepository cr) : base(cr)
        { }

        // POST api/<controller>
        [HttpPost]
        public override async Task<IActionResult> Post([FromBody]Part docIn)
        {
            // make sure required fields are included
            if (docIn.Instrument == null)
            {
                return MissingPropertyResult("instrument");
            }
            if (docIn.Song == null)
            {
                return MissingPropertyResult("song");
            }

            // make sure song exists
            string songsCollection = CollectionNames.SONGS;
            string songId = docIn.Song;
            Song song;
            try
            {
                song = await CosmosRepo.GetDocument<Song>(songsCollection, songId);
            }
            catch (DocumentClientException)
            {
                return ItemNotFoundResult(songId, songsCollection);
            }

            // create part
            Part newDoc = await CosmosRepo.CreateDocument(CollectionName, docIn);
            string partId = newDoc.Id;

            // update song
            if (!song.Parts.Contains(partId))
            {
                List<string> parts = song.Parts.ToList();
                parts.Add(partId);
                song.Parts = parts;
                await CosmosRepo.ReplaceDocument(songsCollection, songId, song);
            }

            return Created(GetGetUri(newDoc.Id), newDoc);
        }

        // PUT api/<controller>/5/rename
        [HttpPut("{partid}/rename")]
        public async Task<IActionResult> Rename(string partid, [FromBody]Part replacement)
        {
            // make sure required fields are included
            if (replacement.Instrument == null)
            {
                return MissingPropertyResult("instrument");
            }

            return await ChangeInDB(partid, part =>
            {
                part.Instrument = replacement.Instrument;
                return part;
            });
        }
    }
}
