using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeCinema.Entities
{
    public class Stock : IEntityBase
    {
        public Stock()
        {
            Rentals = new List<Rental>();
        }

        public int ID { get; set; }
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }
        public Guid Uniquekey { get; set; }
        public bool IsAvailable { get; set; }
        public ICollection<Rental> Rentals { get; set; }
    }
}
