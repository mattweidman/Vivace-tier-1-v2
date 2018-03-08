using System.Collections.Generic;

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
