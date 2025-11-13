using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Modelo
{
    public class Producto
    {
        public int ID { get; set; }

        public string nombre { get; set; } = "";

        public int ramoID { get; set; } 

        public string companiaID { get; set; } = "";
    }
}
