using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vivace.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public IEnumerable<string> Bands { get; set; }
        public IEnumerable<string> Events { get; set; }
    }
}
