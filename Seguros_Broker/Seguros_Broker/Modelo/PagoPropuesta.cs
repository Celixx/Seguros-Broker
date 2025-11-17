using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Modelo
{
    public class PagoPropuesta
    {
        public int ID { get; set; }
        public int PropuestaID { get; set; }
        public int NumeroPoliza { get; set; }
        public int CuotaNro { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string FormaPago { get; set; } = "";
        public string NumeroDocumento { get; set; } = "";
        public string NroTarjCtaCte { get; set; } = "";
        public string TipoTarj { get; set; } = "";
        public string ValidezTarj { get; set; } = "";
        public string Banco { get; set; } = "";
    }
}

