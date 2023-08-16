using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArthouseRepository.Models
{
    public class Artwork
    {
        public int ID { get; set; }
        public string Summary => (ID>0) ? TypeOfArt + " By: " + Artist : "New Artwork";
        public string Completed => "Completed: " + DateFinished.ToString("d");
        public string Title { get; set; }
        public DateTime DateFinished { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public int ArtTypeID { get; set; }
        public string TypeOfArt { get; set; }
        public int ArtistID { get; set; }
        public string Artist { get; set; }
        public Byte[] Timestamp { get; set; }
    }
}
