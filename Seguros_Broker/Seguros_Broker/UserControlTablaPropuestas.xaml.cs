using Seguros_Broker.Modelo;
using Seguros_Broker.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Seguros_Broker
{
    /// <summary>
    /// Interaction logic for UserControlTablaPropuestas.xaml
    /// </summary>
    public partial class UserControlTablaPropuestas : UserControl
    {
        private PropuestaRep propuestaRep = new PropuestaRep();
        private PagosPropuestaRep pagosPropuestaRep = new PagosPropuestaRep();
        public UserControlTablaPropuestas()
        {
            InitializeComponent();

            var propuestas = propuestaRep.GetPropuestas();
            dataGridPropuestas.ItemsSource = propuestas;

            var propuestasPorVencer = propuestasPagoVencer();
            dataGridPagosPropuestas.ItemsSource = propuestasPorVencer;
            if (true) {

            }
        }

        private List<Propuesta> propuestasPagoVencer()
        {
            var planPagos = pagosPropuestaRep.GetPagosByPropuesta();
            var propuestasVencer = new List<Propuesta>();

            foreach (var pago in planPagos)
            {
                if (pago.FechaVencimiento >= DateTime.Now && pago.FechaVencimiento <= DateTime.Now.AddDays(7))
                {
                    var propuestaVencer = propuestaRep.GetPropuesta(pago.PropuestaID);
                    propuestasVencer.Add(propuestaVencer);
                }
            }

            return propuestasVencer;
        }
    }
}
