using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vivace.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace vivace.Controllers
{
    [Route("api/[controller]")]
    public class PartsController : ControllerVivace<Part>
    {
        protected override string COLLECTION_NAME { get { return "Parts"; } }

        protected override string ITEM_NOT_FOUND { get { return "Part ID not found"; } }

        public PartsController(ICosmosRepository cr) : base(cr)
        { }

        // PUT api/<controller>/5/rename
        [HttpPut("{partid}/rename")]
        public async Task<IActionResult> Rename(string partid, [FromBody]Part replacement)
        {
            // The user is supposed to put something like
            // { "instrument": "trombone" } in the body.
            // If the user gives any other properties besides instrument,
            // they will be ignored. 

            return await ChangeInDB(partid, part =>
            {
                part.Instrument = replacement.Instrument;
                return part;
            });
        }
    }
}
