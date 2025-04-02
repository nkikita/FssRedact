using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FssRedact.Models
{
    public class PeriodDate
    {
        
        public DateTime? period_begin {get; set; }
        public DateTime? period_end {get; set; }
        public int? idle_average {get; set; }

    }
}