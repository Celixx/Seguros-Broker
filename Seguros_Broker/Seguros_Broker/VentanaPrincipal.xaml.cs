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
using System.Windows.Shapes;

namespace Seguros_Broker
{
    /// <summary>
    /// Interaction logic for VentanaPrincipal.xaml
    /// </summary>
    public partial class VentanaPrincipal
    {
        public VentanaPrincipal()
        {
            InitializeComponent();
        }

        private void Ejecutivo_Click(object sender, RoutedEventArgs e)
        {
            var VentanaEjecutivo = new Ejecutivo();
            VentanaEjecutivo.ShowDialog();
        }
        private void Socio_Click(object sender, RoutedEventArgs e)
        {
            var VentanaSocio = new SocioMantenedor();
            VentanaSocio.ShowDialog();
        }       

        private void NuevaPropuesta_Click(object sender, RoutedEventArgs e)
        {
            var NuevaPropuestaCaratula = new NuevaPropuestaCaratula();
            NuevaPropuestaCaratula.ShowDialog();
        }

        private void Compania_Click(object sender, RoutedEventArgs e)
        {
            var CompaniaMantenedor = new CompaniaMantenedor();
            CompaniaMantenedor.ShowDialog();
        }

        private void Moneda_Click(object sender, RoutedEventArgs e)
        {
            var VentanaMoneda = new MonedaMantedor();
            VentanaMoneda.ShowDialog();

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Cliente_Click(object sender, RoutedEventArgs e)
        {
            var ClienteMantenedor = new ClienteMantenedor();
            ClienteMantenedor.ShowDialog();
        }

        private void Producto_Click(object sender, RoutedEventArgs e)
        {
            var ProductoMantendor = new ProductoMantenedor();
            ProductoMantendor.ShowDialog();
        }
    }
}
