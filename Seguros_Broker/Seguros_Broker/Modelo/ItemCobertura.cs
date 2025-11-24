using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Modelo
{
    public class ItemCobertura
    {
        public int IdItemCobertura { get; set; }
        public int IdPropuesta { get; set; }       
        public int IdItem { get; set; }            
        public string CodCobertura { get; set; }   
        public string AfectaExenta { get; set; }    
        public string SumaAlMonto { get; set; }     
        public decimal Monto { get; set; }
        public decimal Prima { get; set; }
    }
}
