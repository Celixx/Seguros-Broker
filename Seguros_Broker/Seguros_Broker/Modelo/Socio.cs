using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Modelo
{
    public class Socio
    {
        public string tipoId { get; set; } = "";
        public int ID { get; set; }
        public string nombre { get; set; } = "";
        public string aPaterno { get; set; } = "";
        public string aMaterno { get; set; } = "";
        public int fono { get; set; }
        public int celular { get; set; }
        public string mail { get; set; } = "";
        public int fax { get; set; }
        public string direccion { get; set; } = "";
        public string observacion { get; set; } = "";
        public int comision { get; set; }
        public int porcentajeComision { get; set; }
    }
}
