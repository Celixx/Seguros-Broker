using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Modelo
{
    public class Compania
    {
        public string tipoID { get; set; } = "";
        public string ID { get; set; } = "";
        public string nombre { get; set; } = "";
        public string grupo { get; set; } = "";
        public int fono {  get; set; }
        public string paginaWeb { get; set; } = "";
        public string pais { get; set; } = "";
        public string ciudad { get; set; } = "";
        public string region { get; set; } = "";
        public string comuna { get; set; } = "";
        public string direccion { get; set; } = "";
    }
}
