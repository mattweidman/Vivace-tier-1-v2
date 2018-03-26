using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vivace.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace vivace.Controllers
{
    [Route("api/[controller]")]
    public class PartsController : ControllerVivace<Part>
    {
        public override string COLLECTION_NAME { get { return "Parts"; } }

        protected override string ITEM_NOT_FOUND { get { return "Part ID not found"; } }

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

            return await base.Post(docIn);
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
