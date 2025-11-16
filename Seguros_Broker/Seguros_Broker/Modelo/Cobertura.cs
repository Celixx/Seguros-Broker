using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Modelo
{
    public class Cobertura
    {
        // --- AÑADE ESTA LÍNEA ---
        public bool IsSelected { get; set; }
        

       
        // (Deben ser propiedades { get; set; } para que el DataGrid funcione)
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string afectaExtenta { get; set; }
        public string sumaMonto { get; set; }
        public int monto { get; set; }
        public int prima { get; set; }

        
    }
}
