using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.Dtos
{
    public class WeekDto
    {
        public DateTime FirstDay { get; set; }
        public DateTime LastDay { get; set; }
        public int Number { get; set; }
    }
}
