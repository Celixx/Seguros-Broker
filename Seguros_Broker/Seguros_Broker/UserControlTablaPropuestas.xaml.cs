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
        public UserControlTablaPropuestas()
        {
            InitializeComponent();

            var propuestas = propuestaRep.GetPropuestas();
            dataGridPropuestas.ItemsSource = propuestas;
        }
    }
}
