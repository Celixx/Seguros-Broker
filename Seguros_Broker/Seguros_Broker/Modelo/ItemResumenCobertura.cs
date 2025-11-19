using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Modelo
{
    public class ItemResumenCobertura
    {
        // Mapea a IdItem (lo que el usuario ve como "Número Item")
        public int Numero { get; set; }

        // Mapea a la Prima Neta total del ítem
        public decimal PrimaNeta { get; set; }
    }
}