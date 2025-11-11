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
using Seguros_Broker.Modelo;
using Seguros_Broker.Repositorio;
using System.Text.RegularExpressions;

namespace Seguros_Broker
{

    public partial class MonedaMantedor : Window
    {
        public MonedaMantedor()
        {
            InitializeComponent();

            ReadMoneda();

            LimpiarFormulario();
        }

        private void ReadMoneda()
        {
            var repo = new MonedaRep();
            var monedas = repo.GetMonedas();

            MessageBox.Show($"Monedas Obtenidas: {monedas?.Count ?? 0}");
            this.dataGridMoneda.ItemsSource = monedas;
            

        }

        private void CargarDatosMonedaEnFormulario(Moneda moneda)
        {

            if (moneda == null) return;

            txtNombreMoneda.Text = moneda.nombre.ToString();
            txtSimboloMoneda.Text = moneda.simbolo.ToString();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var monedaSeleccionada = (Moneda)dataGridMoneda.SelectedItem;

            if (monedaSeleccionada != null)
            {
                CargarDatosMonedaEnFormulario(monedaSeleccionada);
            }
        }


        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            int idBuscado = int.Parse(txtBuscar.Text);

            //if (string.IsNullOrWhiteSpace(idBuscado))
            //{
            //    MessageBox.Show("Por favor, ingrese un ID para buscar.", "Entrada Requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            MonedaRep repository = new MonedaRep();
            Moneda? monedaEncontrada = repository.GetMoneda(idBuscado);

            if (monedaEncontrada != null)
            {

                CargarDatosMonedaEnFormulario(monedaEncontrada);

                MessageBox.Show($"Ejecutivo encontrado: {monedaEncontrada.nombre} ", "Búsqueda Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"No se encontró ningún ejecutivo con el ID: {idBuscado}", "No Encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LimpiarFormulario()
        {
            txtNombreMoneda.Clear();
            txtBuscar.Clear();
            txtSimboloMoneda.Clear();

        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
          var errores = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(txtNombreMoneda.Text))
                errores.Add("Nombre Obligatorio");

            else if (txtNombreMoneda.Text.Any(char.IsDigit))
            {
                errores.Add("El nombre no puede contener números.");
            }

            if (errores.Any())
            {
                MessageBox.Show("Corrija los siguientes errores:\n- " + string.Join("\n- ", errores), "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nuevaMoneda = new Moneda
            {
                monedaId = 0,
                nombre = txtNombreMoneda.Text.Trim(),
                simbolo = txtSimboloMoneda.Text.Trim()
            };

            var repo = new MonedaRep();

            var result = await repo.CreateMonedaAsync(nuevaMoneda);

            if (result.success)
            {
                MessageBox.Show("Moneda guardada correctamente. ", "Exito", MessageBoxButton.OK, MessageBoxImage.Information);

                ReadMoneda();

                LimpiarFormulario();
            }
            else 
            {
                MessageBox.Show("No se pudo guardar: " + (result.errorMessage ?? "Error desconocido"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Seguro que quiere salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }
}
