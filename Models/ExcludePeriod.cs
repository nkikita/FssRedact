using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FssRedact.Models
{
    public class ExcludePeriod
    {
        
        public int? period_type {get; set; }
        public DateTime? begin_date {get; set; }
        public DateTime? end_date {get; set; }
    }
}