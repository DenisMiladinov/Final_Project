using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class VacationSpotCategory
    {
        public int VacationSpotId { get; set; }
        public VacationSpot VacationSpot { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
