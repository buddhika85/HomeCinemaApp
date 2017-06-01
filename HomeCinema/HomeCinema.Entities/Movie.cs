using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeCinema.Entities
{
    public class Movie : IEntityBase
    {
        public Movie()
        {
            Stocks = new List<Stock>();
        }

        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; }
        public string Deirector { get; set; }
        public string Writer { get; set; }
        public string producer { get; set; }
        public DateTime ReleaseDate { get; set; }
        public byte Rating { get; set; }
        public string TrailerURL { get; set; }
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
