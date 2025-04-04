using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FssRedact.Models
{
    public class Proactive_documents
    {
        public int Id { get; set; }  
        public string Dataxml { get; set; } = string.Empty; 
        public DateTime Datecreated { get; set; } = DateTime.Now; 
        public DateTime Datemodified { get; set; } = DateTime.Now; 
        public string Namecreated { get; set; } = string.Empty; 
        public string Namemodified { get; set; } = string.Empty; 
    
        
    }
}