using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FssRedact.Models
{
    public class PeriodDate
    {
        
        public DateTime? Period_begin {get; set; }
        public DateTime? Period_end {get; set; }
        public int? Idle_average {get; set; }

    }
}