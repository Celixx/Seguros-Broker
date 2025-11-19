using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.ComponentModel; 

namespace Seguros_Broker.Modelo
{
    public class Item
    {
       
        public int IdItem { get; set; }

       
        public string RutCliente { get; set; }

        public string MateriaAsegurada { get; set; }
        public string Anno { get; set; }
        public string Patente { get; set; }
        public string MinutaItem { get; set; }
        public string Carroceria { get; set; }
        public string Propietario { get; set; }
        public string Tipo { get; set; }
        public string NumeroMotor { get; set; }
        public string Color { get; set; }
        public string Chasis { get; set; }
        public string ValorComercial { get; set; }
        public string Modelo { get; set; }
        public string NumeroChasis { get; set; }
        public string Uso { get; set; }

        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }
}