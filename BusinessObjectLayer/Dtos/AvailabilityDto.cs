using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.Dtos
{
    public class AvailabilityDto
    {
        public int Id { get; set; }
        public string UserUsername { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double AvailableHours { get; set; }
    }
}
