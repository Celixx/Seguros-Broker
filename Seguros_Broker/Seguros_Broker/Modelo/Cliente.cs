using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Modelo
{
    public class Cliente
    {
        public string TipoIdentificacion { get; set; } = "";
        public string ID { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string ApellidoPaterno { get; set; } = "";
        public string ApellidoMaterno { get; set; } = "";

        public int HoldingID { get; set; } = 0;
        public string HoldingNombre { get; set; } = "";

        public string EjecutivoID { get; set; } = "";
        public string EjecutivoNombre { get; set; } = "";

        public string Fonos { get; set; } = "";
        public string PaginaWeb { get; set; } = "";
        public string NombreCorto { get; set; } = "";
        public string Referencia { get; set; } = "";

        // Dirección Particular
        public string Particular_Pais { get; set; } = "DESCONOCIDO";
        public string Particular_Region { get; set; } = "";
        public string Particular_Ciudad { get; set; } = "";
        public string Particular_Comuna { get; set; } = "";
        public string Particular_Direccion { get; set; } = "";

        // Dirección Comercial
        public string Comercial_Pais { get; set; } = "DESCONOCIDO";
        public string Comercial_Region { get; set; } = "";
        public string Comercial_Ciudad { get; set; } = "";
        public string Comercial_Comuna { get; set; } = "";
        public string Comercial_Direccion { get; set; } = "";
    }
}
