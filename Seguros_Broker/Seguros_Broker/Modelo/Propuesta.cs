using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Modelo
{
    public class Propuesta
    {
        public int ID;
        public int NumeroPoliza {  get; set; }
        public int RenuevaPoliza { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public string TipoPoliza { get; set; } = "";
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaEmision { get; set; }
        public int IDRamo { get; set; }
        public string IDEjecutivo { get; set; } = "";
        public string Area {  get; set; } = "";
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaVigenciaDesde { get; set; }
        public DateTime? FechaVigenciaHasta { get; set; }
        public int IDMoneda { get; set; }
        public int ComisionAfecta { get; set; }
        public int ComisionExenta { get; set; }
        public int MontoAsegurado { get; set; }
        public int ComisionTotal { get; set; }
        public int PrimaNetaAfecta { get; set; }
        public int PrimaNetaExenta { get; set; }
        public int PrimaNetaTotal { get; set; }
        public int IVA { get; set;}
        public int PrimaBrutaTotal { get; set; }
        public string IDCliente { get; set; } = "";
        public int IDSocio { get; set; }
        public int IDGestor { get; set; }
        public string IDCompania { get; set; } = "";
        public string MateriaAsegurada { get; set; } = "";
        public string Observacion { get; set; } = "";
    }
}
