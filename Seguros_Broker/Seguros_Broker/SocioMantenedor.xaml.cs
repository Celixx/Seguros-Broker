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
using System.Windows.Shapes;

namespace Seguros_Broker
{
    /// <summary>
    /// Interaction logic for SocioMantenedor.xaml
    /// </summary>
    public partial class SocioMantenedor : Window
    {
        private SocioRep socioRep = new SocioRep();
        public SocioMantenedor()
        {
            InitializeComponent();

            ReadSocio();
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Seguro que quiere salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btnActualizar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void txtCelular_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ReadSocio()
        {
             var socios = socioRep.GetSocios();

            this.dataGridSocio.ItemsSource = socios;
        }

        private void LimpiarFormulario()
        {
            cbTipoIdentificacion.SelectedIndex = 0;
            txtIdentificacion.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtApaterno.Text = string.Empty;
            txtAmaterno.Text = string.Empty;
            txtFono.Text = string.Empty;
            txtCelular.Text = string.Empty;
            txtMail.Text = string.Empty;
            txtFax.Text = string.Empty;
            txtDireccion.Text = string.Empty;
            cbNivelComision.SelectedIndex = 0;
            txtPorcentajeComision.Text = string.Empty;
        }
    }
}
