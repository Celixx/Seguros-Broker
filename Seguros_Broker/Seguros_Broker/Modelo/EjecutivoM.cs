using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Modelo
{
    public class EjecutivoM
    {
        public int codigo { get; set; }
        public string tipoId { get; set; } = "";
        public int ID { get; set; }
        public string nombre { get; set; } = "";
        public string aPaterno { get; set; } = "";
        public string aMaterno { get; set; } = "";
        public int fono { get; set; }      
        public int celular { get; set; }
        public string mail { get; set; } = "";
        public int comision { get; set; }
        public string nick { get; set; } = "";
        public string perfil { get; set; } = "";
        public string estado { get; set; } = "";
        public string restricciones { get; set; } = "";
    }
}
