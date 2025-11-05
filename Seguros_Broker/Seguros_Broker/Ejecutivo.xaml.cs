using Seguros_Broker.Modelo;
using Seguros_Broker.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
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

    public partial class Ejecutivo : Window
    {
        

        public Ejecutivo()
        {
            InitializeComponent();

            ReadEjecutivo();
            
        }

        private void ReadEjecutivo()
        {
            var repo = new EjecutivoRep();
            var ejecutivos = repo.GetEjecutivos();

            MessageBox.Show($"Ejecutivos obtenidos: {ejecutivos?.Count ?? 0}");
            this.dataGridEjecutivo.ItemsSource = ejecutivos;
        }



        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // --- Validaciones ---
            var errores = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
                errores.Add("Código (obligatorio).");

            if (cbTipoIdentificacion.SelectedItem == null ||
                ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString().ToUpper().Contains("SELECCIONE"))
                errores.Add("Tipo identificación (obligatorio).");

            if (string.IsNullOrWhiteSpace(txtIdentificacion.Text))
                errores.Add("Identificación (obligatorio).");

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                errores.Add("Nombre (obligatorio).");

            if (string.IsNullOrWhiteSpace(txtMail.Text))
                errores.Add("Mail (obligatorio).");

            if (cbNivelComision.SelectedItem == null)
                errores.Add("Nivel de comisión (obligatorio).");

            if (string.IsNullOrWhiteSpace(txtNick.Text))
                errores.Add("Nick (obligatorio).");

            if (cbPerfil.SelectedItem == null ||
                ((ComboBoxItem)cbPerfil.SelectedItem).Content.ToString().ToUpper().Contains("SELECCIONE"))
                errores.Add("Perfil (obligatorio).");

            if (cbEstado.SelectedItem == null ||
                ((ComboBoxItem)cbEstado.SelectedItem).Content.ToString().ToUpper().Contains("SELECCIONE"))
                errores.Add("Estado (obligatorio).");

            if (cbRestricciones.SelectedItem == null)
                errores.Add("Restricciones (obligatorio).");

            // Email simple
            if (!string.IsNullOrWhiteSpace(txtMail.Text) && !IsValidEmail(txtMail.Text))
                errores.Add("Mail inválido.");

            if (!int.TryParse(txtCodigo.Text, out int codigoParsed))
                errores.Add("Código debe ser numérico.");

            if (!int.TryParse(txtIdentificacion.Text, out int identificacionParsed))
                errores.Add("Identificación debe ser numérica.");

            if (errores.Any())
            {
                MessageBox.Show("Corrija los siguientes errores:\n- " + string.Join("\n- ", errores), "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // --- Mapeo de modelo ---
            var nuevo = new EjecutivoM
            {
                codigo = codigoParsed,
                tipoId = ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString(),
                ID = identificacionParsed,
                nombre = txtNombre.Text.Trim(),
                aPaterno = txtApePaterno.Text?.Trim() ?? "",
                aMaterno = txtApeMaterno.Text?.Trim() ?? "",
                fono = int.TryParse(txtFono.Text, out int f) ? f : 0,
                celular = int.TryParse(txtCelular.Text, out int c) ? c : 0,
                mail = txtMail.Text.Trim(),
                comision = ParseNivelComision(((ComboBoxItem)cbNivelComision.SelectedItem).Content.ToString()),
                nick = txtNick.Text.Trim(),
                perfil = ((ComboBoxItem)cbPerfil.SelectedItem).Content.ToString(),
                estado = ((ComboBoxItem)cbEstado.SelectedItem).Content.ToString(),
                restricciones = ((ComboBoxItem)cbRestricciones.SelectedItem).Content.ToString()
            };

            // --- Llamada async al repositorio ---
            var repo = new EjecutivoRep();

            // Evitamos la deconstrucción; en su lugar usamos 'result'
            var result = await repo.CreateEjecutivoAsync(nuevo);

            if (result.success)
            {
                MessageBox.Show("Ejecutivo guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refrescar la grilla
                ReadEjecutivo();

                // Limpiar formulario
                LimpiarFormulario();
            }
            else
            {
                // Mostrar mensaje de error proveniente del repositorio
                MessageBox.Show("No se pudo guardar: " + (result.errorMessage ?? "Error desconocido"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- Métodos auxiliares ---
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

        private static int ParseNivelComision(string contenidoCombo)
        {
            if (string.IsNullOrWhiteSpace(contenidoCombo)) return 0;
            var m = Regex.Match(contenidoCombo, @"\d+");
            if (m.Success && int.TryParse(m.Value, out int v)) return v;
            return 0;
        }

        private void LimpiarFormulario()
        {
            txtCodigo.Clear();
            cbTipoIdentificacion.SelectedIndex = 0;
            txtIdentificacion.Clear();
            txtNombre.Clear();
            txtApePaterno.Clear();
            txtApeMaterno.Clear();
            txtFono.Clear();
            txtCelular.Clear();
            txtMail.Clear();
            cbNivelComision.SelectedIndex = 0;
            txtPorcentajeComision.Clear();
            txtNick.Clear();
            cbPerfil.SelectedIndex = 0;
            cbEstado.SelectedIndex = 0;
            cbRestricciones.SelectedIndex = 0;
        }
        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
