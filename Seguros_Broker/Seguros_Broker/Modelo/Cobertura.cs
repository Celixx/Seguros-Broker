using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Modelo
{
    public class Cobertura
    {
        public string codigo { get; set; } = "";

        public string nombre { get; set; } = "";

        public string afectaExtenta { get; set; } = "";

        public string sumaMonto { get; set; } = "";

        public int monto { get; set; }

        public int prima { get; set; }

    }

}
