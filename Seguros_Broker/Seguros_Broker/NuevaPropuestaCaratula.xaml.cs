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
using System.Windows.Shapes;

namespace Seguros_Broker
{
    /// <summary>
    /// Interaction logic for VentanaPrincipal.xaml
    /// </summary>
    public partial class NuevaPropuestaCaratula
    {

        private List<Moneda> monedas;
        private MonedaRep monedaRep= new MonedaRep();

        public NuevaPropuestaCaratula()
        {
            InitializeComponent();

            this.monedas = monedaRep.GetMonedas();

            cbMonedas.ItemsSource = monedas;
        }

        private List<Modelo.EjecutivoM> GetEjecutivo()
        {
            var repo = new EjecutivoRep();
            var ejecutivos = repo.GetEjecutivos();
            return ejecutivos;
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnBuscarRamo_Click(object sender, RoutedEventArgs e)
        {
            var ramoRep = new RamoRep();
            var ramoBuscado =  ramoRep.GetRamo(int.Parse(TxtCodigoRamo.Text));

            TxtRamo.Visibility = Visibility.Visible;
            TxtRamo.Text = ramoBuscado.Nombre;
            return;
        }


        private void BtnBuscarEjecutivoCuenta_Click(object sender, RoutedEventArgs e)
        {
            var ejecutivos = GetEjecutivo();

            foreach (var ejecutivo in ejecutivos)
            {
                if (ejecutivo.codigo.ToString() == TxtCodigoEjecutivo.Text)
                {
                    TxtEjecutivoCuenta.Visibility = Visibility.Visible;
                    TxtEjecutivoCuenta.Text = ejecutivo.nombre + " " + ejecutivo.aPaterno + " " + ejecutivo.aMaterno;
                    return;
                }
            }

            MessageBox.Show($"No se encontró ningún ejecutivo con el ID: {TxtCodigoEjecutivo.Text}");
        }

        private void BtnBuscarEjecutivoResponsable_Click(object sender, RoutedEventArgs e)
        {
            var ejecutivos = GetEjecutivo();

            foreach (var ejecutivo in ejecutivos)
            {
                if (ejecutivo.codigo.ToString() == TxtCodigoEjecutivoResponsable.Text)
                {
                    TxtEjecutivoResponsable.Visibility = Visibility.Visible;
                    TxtEjecutivoResponsable.Text = ejecutivo.nombre + " " + ejecutivo.aPaterno + " " + ejecutivo.aMaterno;
                    return;                    
                }
            }

            MessageBox.Show($"No se encontró ningún ejecutivo con el ID: {TxtCodigoEjecutivoResponsable.Text}");
        }
        private void BtnBuscarArea_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnBuscarRutFacturar_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnBuscarRutContratante_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void BtnBuscarRutAsegurado_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnBuscarRutAFavorDe_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void BtnBuscarRutSocio_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void BtnBuscarRutGestor_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void BtnBuscarRutCompania_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void BtnLimpiarEjecutivo_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnAgregarItem(object sender, RoutedEventArgs e)
        {
            var VentanaAgregarNuevoItem = new VentanaAgregarNuevoItem();
            VentanaAgregarNuevoItem.ShowDialog();
        }

        private void cbFormatoHoja_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnPdf_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TxtCodigoRamo_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BtnBuscarMoneda_Click(object sender, RoutedEventArgs e)
        {
            TxtMontoAsegurado.Text = "0,00";
            TxtComisionTotal.Text = "379,80";
            if (TxtComisiónAfectaPorcentaje.Text == "")
            {
                TxtMontoAsegurado.Text = "0,00";
            }
            else
            {
                TxtMontoAsegurado.Text = "1,00";
            }
        }
    }

}
