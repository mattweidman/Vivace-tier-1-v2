﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vivace.Models;

namespace vivace.Controllers
{
    [Route("api/[controller]")]
    public class BandsController : ControllerVivace<Band>
    {
        public override string CollectionName { get { return CollectionNames.BANDS; } }

        public BandsController(ICosmosRepository cr) : base(cr)
        { }

        // POST api/<controller>
        [HttpPost]
        public override async Task<IActionResult> Post([FromBody]Band docIn)
        {
            if (docIn.Name == null)
            {
                return MissingPropertyResult("name");
            }

            docIn.Users = new List<string>();
            docIn.Songs = new List<string>();
            docIn.Events = new List<string>();

            return await base.Post(docIn);
        }

        // PUT api/<controller>/5/addsong/5
        [HttpPut("{bandid}/addsong/{songid}")]
        public async Task<IActionResult> AddSong(string bandid, string songid)
        {
            // check that song exists
            string otherCollection = CollectionNames.SONGS;
            Song song;
            try
            {
                song = await CosmosRepo.GetDocument<Song>(otherCollection, songid);
            }
            catch (DocumentClientException)
            {
                return ItemNotFoundResult(songid, otherCollection);
            }

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
            // make sure name field exists
            if (replacement.Name == null)
            {
                return MissingPropertyResult("name");
            }

            return await ChangeInDB(bandid, band =>
            {
                band.Name = replacement.Name;
                return band;
            });
        }
    }
}
