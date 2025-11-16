using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Modelo
{
    public class ClienteDatosPago
    {
        public int ID { get; set; }
        public string ClienteID { get; set; } = "";
        public string NumeroDocumento { get; set; } = "";
        public string NroTarjetaCuenta { get; set; } = "";
        public string TipoTarjeta { get; set; } = "";
        public string ValidezTarjeta { get; set; } = "";
        public string Banco { get; set; } = "";
    }
}

