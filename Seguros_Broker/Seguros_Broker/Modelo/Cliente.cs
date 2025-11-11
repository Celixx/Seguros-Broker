using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Modelo
{
    public class Cliente
    {
        // Identificación
        public string tipoIdentificacion { get; set; } = "";
        public string ID { get; set; } = ""; 
        public string nombre { get; set; } = "";
        public string aPaterno { get; set; } = "";
        public string aMaterno { get; set; } = "";
        public string holding { get; set; } = "";
        public string ejecutivoID { get; set; } = ""; 
        public string fonos { get; set; } = "";
        public string paginaWeb { get; set; } = "";
        public string nombreCorto { get; set; } = "";
        public string referencia { get; set; } = "";

        // Particular
        public string particularPais { get; set; } = "";
        public string particularRegion { get; set; } = "";
        public string particularCiudad { get; set; } = "";
        public string particularComuna { get; set; } = "";
        public string particularDireccion { get; set; } = "";

        // Comercial
        public string comercialPais { get; set; } = "";
        public string comercialRegion { get; set; } = "";
        public string comercialCiudad { get; set; } = "";
        public string comercialComuna { get; set; } = "";
        public string comercialDireccion { get; set; } = "";
    }
}
