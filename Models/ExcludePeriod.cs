using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FssRedact.Models
{
    public class ExcludePeriod
    {
        
        public int? Period_type {get; set; }
        public DateTime? Begin_date {get; set; }
        public DateTime? End_date {get; set; }
    }
}