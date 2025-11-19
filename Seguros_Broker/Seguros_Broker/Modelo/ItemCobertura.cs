using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Modelo
{
    public class ItemCobertura
    {
        // Campos de la clave primaria (PK) y Foráneas (FK)
        public int IdItemCobertura { get; set; }
        public int IdPropuesta { get; set; }        // FK a PROPUESTA(ID)
        public int IdItem { get; set; }             // FK a Item(IdItem)
        public string CodCobertura { get; set; }    // FK a COBERTURA(Codigo)

        // Campos Económicos y de Configuración
        public string AfectaExenta { get; set; }    // Corresponde a afectaExenta (VARCHAR)
        public string SumaAlMonto { get; set; }     // Corresponde a SumaAlMonto (VARCHAR)
        public decimal Monto { get; set; }
        public decimal Prima { get; set; }
    }
}
