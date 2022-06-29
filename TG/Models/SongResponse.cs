using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TG.Models
{
    public class SongResponse
    {
        public SongResponse(string title, string artistName, string uRL)
        {
            Title = title;
            ArtistName = artistName;
            URL = uRL;
        }

        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string URL { get; set; }
        public List<string> genres { get; set; }
        public string genre { get; set; }
        public string difficulty { get; set; }
        public string YoutubeURL { get; set; }

    }
}
