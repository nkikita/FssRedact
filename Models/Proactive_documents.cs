using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FssRedact.Models
{
    public class Proactive_documents
    {
        public int id { get; set; }  
        public string dataxml { get; set; } = string.Empty; 
        public DateTime datecreated { get; set; } = DateTime.Now; 
        public DateTime datemodified { get; set; } = DateTime.Now; 
        public string namecreated { get; set; } = string.Empty; 
        public string namemodified { get; set; } = string.Empty; 
    
        
    }
}