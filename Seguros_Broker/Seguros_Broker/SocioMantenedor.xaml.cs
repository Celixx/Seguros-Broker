using Seguros_Broker.Modelo;
using Seguros_Broker.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            int idBuscado = int.Parse(txtSearch.Text);

            if (idBuscado == null)
            {
                MessageBox.Show("Por favor, ingrese un ID para buscar.", "Entrada Requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Socio? socioEncontrado = socioRep.GetSocio(idBuscado);

            if (socioEncontrado != null)
            {

                CargarDatosSocioEnFormulario(socioEncontrado);

                MessageBox.Show($"Ejecutivo encontrado: {socioEncontrado.nombre} {socioEncontrado.aPaterno}", "Búsqueda Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"No se encontró ningún ejecutivo con el ID: {idBuscado}", "No Encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CargarDatosSocioEnFormulario(Socio socio)
        {

            if (socio == null) return;

            cbTipoIdentificacion.Text = socio.tipoId;
            txtIdentificacion.Text = socio.ID.ToString();
            txtNombre.Text = socio.nombre;
            txtApaterno.Text = socio.aPaterno;
            txtAmaterno.Text = socio.aMaterno;
            txtFono.Text = socio.fono.ToString();
            txtCelular.Text = socio.celular.ToString();
            txtMail.Text = socio.mail;
            txtFax.Text = socio.fax.ToString();
            txtDireccion.Text = socio.direccion;
            txtObservacion.Text = socio.observacion;
            cbNivelComision.Text = GetTextoComision(socio.comision);
            txtPorcentajeComision.Text = socio.porcentajeComision.ToString();
        }

        private string GetTextoComision(int comisionValor)
        {
            switch (comisionValor)
            {
                case 5:
                    return "5(NIVEL POR DEFECTO)";
                case 10:
                    return "10";
                case 15:
                    return "15";
                default:

                    return "5(NIVEL POR DEFECTO)";
            }
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            var errores = new System.Collections.Generic.List<string>();

            int codigoParsed = 0;

            if (cbTipoIdentificacion.SelectedItem == null || ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString().ToUpper().Contains("SELECCIONE"))
                errores.Add("Tipo identificación (obligatorio).");

            if (string.IsNullOrWhiteSpace(txtIdentificacion.Text))
                errores.Add("Identificación (obligatorio).");


            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                errores.Add("Nombre (obligatorio).");

            else if (txtNombre.Text.Any(char.IsDigit))
            {
                errores.Add("El nombre no puede contener números.");
            }

            if (!string.IsNullOrWhiteSpace(txtMail.Text) && !IsValidEmail(txtMail.Text))
                errores.Add("Mail inválido.");

            if (errores.Any())
            {
                MessageBox.Show("Corrija los siguientes errores:\n- " + string.Join("\n- ", errores), "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nuevoSocio = new Socio()
            {
                tipoId = ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString(),
                ID = int.TryParse(txtIdentificacion.Text, out int f) ? f : 0,
                nombre = txtNombre.Text.Trim(),
                aPaterno = txtApaterno.Text.Trim(),
                aMaterno = txtAmaterno.Text.Trim(),
                fono = int.TryParse(txtFono.Text, out int v) ? v : 0,
                celular = int.TryParse(txtCelular.Text, out int b) ? b : 0,
                mail = txtMail.Text.Trim(),
                fax = int.TryParse(txtFax.Text,out int m) ? m : 0,
                direccion = txtDireccion.Text.Trim(),
                observacion = txtObservacion.Text.Trim(),
                comision = ParseNivelComision(((ComboBoxItem)cbNivelComision.SelectedItem).Content.ToString()),
                porcentajeComision = int.TryParse(txtPorcentajeComision.Text, out int h) ? h : 0,
            };

            var result = await socioRep.CreateSocioAsync(nuevoSocio);

            if (result.success)
            {
                MessageBox.Show("Socio guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refrescar la grilla
                ReadSocio();

                // Limpiar formulario
                LimpiarFormulario();
            }
            else
            {
                // Msj de error proveniente del repositorio
                MessageBox.Show("No se pudo guardar: " + (result.errorMessage ?? "Error desconocido"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static int ParseNivelComision(string contenidoCombo)
        {
            if (string.IsNullOrWhiteSpace(contenidoCombo)) return 0;
            var m = Regex.Match(contenidoCombo, @"\d+");
            if (m.Success && int.TryParse(m.Value, out int v)) return v;
            return 0;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase);
            }
            catch { return false; }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Seguro que quiere salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private async void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            var errores = new System.Collections.Generic.List<string>();

            if (cbTipoIdentificacion.SelectedItem == null ||
                ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString().ToUpper().Contains("SELECCIONE"))
                errores.Add("Tipo identificación (obligatorio).");

            if (string.IsNullOrWhiteSpace(txtIdentificacion.Text))
                errores.Add("Identificación (obligatorio).");

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                errores.Add("Nombre (obligatorio).");

            if (errores.Any())
            {
                MessageBox.Show("Corrija los siguientes errores:\n- " + string.Join("\n- ", errores), "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nuevoSocio = new Socio
            {
                tipoId = ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString(),
                ID = int.TryParse(txtIdentificacion.Text, out int f) ? f : 0,
                nombre = txtNombre.Text.Trim(),
                aPaterno = txtApaterno.Text.Trim(),
                aMaterno = txtAmaterno.Text.Trim(),
                fono = int.TryParse(txtFono.Text, out int v) ? v : 0,
                celular = int.TryParse(txtCelular.Text, out int b) ? b : 0,
                mail = txtMail.Text.Trim(),
                fax = int.TryParse(txtFax.Text, out int m) ? m : 0,
                direccion = txtDireccion.Text.Trim(),
                observacion = txtObservacion.Text.Trim(),
                comision = ParseNivelComision(((ComboBoxItem)cbNivelComision.SelectedItem).Content.ToString()),
                porcentajeComision = int.TryParse(txtPorcentajeComision.Text, out int h) ? h : 0,
            };

            var result = await socioRep.UpdateSocioAsync(nuevoSocio);

            if (result.success)
            {
                MessageBox.Show("Socio actualizada correctramente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                //Refrescar Grid
                ReadSocio();

                //Limpiar Form
                LimpiarFormulario();
            }
            else
            {
                //Mostrar mensaje de error del repo
                MessageBox.Show("No se pudo actualizar: " + (result.errorMessage ?? "Error desconocido"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var socioSeleccionado = (Socio)dataGridSocio.SelectedItem;

            if (socioSeleccionado != null)
            {

                CargarDatosSocioEnFormulario(socioSeleccionado);
            }
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

        private void cbTipoIdentificacion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
