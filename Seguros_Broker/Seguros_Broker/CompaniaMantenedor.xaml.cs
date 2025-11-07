using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Seguros_Broker
{
    /// <summary>
    /// Interaction logic for CompaniaMantenedor.xaml
    /// </summary>
    public partial class CompaniaMantenedor : Window
    {
        public CompaniaMantenedor()
        {
            InitializeComponent();

            ReadCompania();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones
            var errores = new System.Collections.Generic.List<string>();

            if (cbTipoIdentificacion.SelectedItem == null ||
                ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString().ToUpper().Contains("SELECCIONE"))
                errores.Add("Tipo identificación (obligatorio).");

            if (string.IsNullOrWhiteSpace(txtIdentificacion.Text))
                errores.Add("Identificación (obligatorio).");

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                errores.Add("Nombre (obligatorio).");

            if (cbGrupo.SelectedItem == null ||
                ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString().ToUpper().Contains("SELECCIONE"))
                errores.Add("Tipo identificación (obligatorio).");

            if (errores.Any())
            {
                MessageBox.Show("Corrija los siguientes errores:\n- " + string.Join("\n- ", errores), "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Mapeo
            var nuevaCompania = new Compania{
                tipoID = ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString(),
                ID = txtIdentificacion.Text.Trim(),
                nombre = txtNombre.Text.Trim(),
                grupo = ((ComboBoxItem)cbGrupo.SelectedItem).Content.ToString(),
                fono = int.TryParse(txtFono.Text, out int f) ? f : 0,
                paginaWeb = txtPagina.Text?.Trim() ?? "",
                pais = txtPais.Text?.Trim() ?? "",
                ciudad = txtCiudad.Text?.Trim() ?? "",
                region = txtRegion.Text?.Trim() ?? "",
                comuna = txtComuna.Text?.Trim() ?? "",
                direccion = txtDireccion.Text?.Trim() ?? ""
            };

            var repo = new CompaniaRep();

            var result = await repo.CreateCompaniaAsync(nuevaCompania);

            if (result.success)
            {
                MessageBox.Show("Compañía guardada correctramente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                //Refrescar Grid
                ReadCompania();

                //Limpiar Form
                LimpiarFormulario();
            }
            else
            {
                //Mostrar mensaje de error del repo
                MessageBox.Show("No se pudo guardar: " + (result.errorMessage ?? "Error desconocido"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void LimpiarFormulario()
        {
            cbTipoIdentificacion.SelectedIndex = 0;
            txtIdentificacion.Clear();
            txtNombre.Clear();
            cbGrupo.SelectedIndex = 0;
            txtFono.Clear();
            txtPagina.Clear();
            txtPais.Clear();
            txtCiudad.Clear();
            txtRegion.Clear();
            txtComuna.Clear();
            txtDireccion.Clear();
        }

        private void ReadCompania()
        {
            var repo = new CompaniaRep();
            var companias = repo.GetCompanias();

            this.dataGridCompania.ItemsSource = companias;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnCancelar_Grupos_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnGuardar_Grupos_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
