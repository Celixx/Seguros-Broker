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

            this.dataGridEjecutivo.ItemsSource = ejecutivos;
        }



        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones
            var errores = new System.Collections.Generic.List<string>();

            int codigoParsed = 0;

            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
                errores.Add("Código (obligatorio).");

            if (cbTipoIdentificacion.SelectedItem == null ||((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString().ToUpper().Contains("SELECCIONE"))
                errores.Add("Tipo identificación (obligatorio).");

            if (string.IsNullOrWhiteSpace(txtIdentificacion.Text))
                errores.Add("Identificación (obligatorio).");


            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                errores.Add("Nombre (obligatorio).");

            else if (txtNombre.Text.Any(char.IsDigit))
            {
                errores.Add("El nombre no puede contener números.");
            }

            if (cbRestricciones.SelectedItem == null)
                errores.Add("Restricciones (obligatorio).");

            if (!string.IsNullOrWhiteSpace(txtMail.Text) && !IsValidEmail(txtMail.Text))
                errores.Add("Mail inválido.");

            if (!string.IsNullOrWhiteSpace(txtCodigo.Text) && !int.TryParse(txtCodigo.Text, out codigoParsed))
                errores.Add("Código debe ser numérico.");
         
            if (!int.TryParse(txtIdentificacion.Text, out int identificacionParsed))
                errores.Add("Identificación debe ser numérica.");

            if (errores.Any())
            {
                MessageBox.Show("Corrija los siguientes errores:\n- " + string.Join("\n- ", errores), "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Mapeo de modelo 
            var nuevo = new EjecutivoM
            {
                codigo = codigoParsed, 
                tipoId = ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString(),
                ID = txtIdentificacion.Text.Trim(),
                nombre = txtNombre.Text.Trim(),
                aPaterno = txtApePaterno.Text?.Trim() ?? "",
                aMaterno = txtApeMaterno.Text?.Trim() ?? "",
                fono = int.TryParse(txtFono.Text, out int f) ? f : 0,
                celular = int.TryParse(txtCelular.Text, out int c) ? c : 0,
                mail = txtMail.Text.Trim(),
                comision = ParseNivelComision(((ComboBoxItem)cbNivelComision.SelectedItem).Content.ToString()),
                porcentajeComision = int.TryParse(txtPorcentajeComision.Text, out int h) ? h : 0,
                nick = txtNick.Text.Trim(),
                perfil = ((ComboBoxItem)cbPerfil.SelectedItem).Content.ToString(),
                estado = ((ComboBoxItem)cbEstado.SelectedItem).Content.ToString(),
                restricciones = ((ComboBoxItem)cbRestricciones.SelectedItem).Content.ToString()
            };


            var repo = new EjecutivoRep();


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
                // Msj de error proveniente del repositorio
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

        private void CargarDatosEjecutivoEnFormulario(EjecutivoM ejecutivo)
        {
  
            if (ejecutivo == null) return;

            txtCodigo.Text = ejecutivo.codigo.ToString();
            txtIdentificacion.Text = ejecutivo.ID; 
            txtNombre.Text = ejecutivo.nombre;
            txtApePaterno.Text = ejecutivo.aPaterno;
            txtApeMaterno.Text = ejecutivo.aMaterno;
            txtFono.Text = ejecutivo.fono.ToString();
            txtCelular.Text = ejecutivo.celular.ToString();
            txtMail.Text = ejecutivo.mail;
            txtNick.Text = ejecutivo.nick;
            txtPorcentajeComision.Text = ejecutivo.porcentajeComision.ToString();
            cbTipoIdentificacion.Text = ejecutivo.tipoId;
            cbPerfil.Text = ejecutivo.perfil;
            cbEstado.Text = ejecutivo.estado;
            cbRestricciones.Text = ejecutivo.restricciones;
            cbNivelComision.Text = GetTextoComision(ejecutivo.comision);
        }

        private async void btnEditar_Click(object sender, RoutedEventArgs e)
        {

            var errores = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
                errores.Add("Código (obligatorio).");

            if (cbTipoIdentificacion.SelectedItem == null || ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString().ToUpper().Contains("SELECCIONE"))
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


            var nuevoEjecutivo = new EjecutivoM
            {
                
                ID = txtIdentificacion.Text.Trim(),
                codigo = int.TryParse(txtCodigo.Text, out int f) ? f : 0,
                nombre = txtNombre.Text.Trim(),
                aPaterno = txtApePaterno.Text.Trim(),
                aMaterno = txtApeMaterno.Text.Trim(),
                fono = int.TryParse(txtFono.Text, out int g) ? g : 0,
                celular = int.TryParse(txtCelular.Text, out int h) ? h : 0,
                mail = txtMail.Text.Trim(),
                nick = txtNick.Text.Trim(),
                porcentajeComision = int.TryParse(txtPorcentajeComision.Text, out int j) ? j : 0,
                tipoId = cbTipoIdentificacion.Text,
                comision = ParseNivelComision(cbNivelComision.Text),
                perfil = cbPerfil.Text,
                estado = cbEstado.Text,
                restricciones = cbRestricciones.Text
            };

            var repo = new EjecutivoRep();

            // Llamada al método Update
            var result = await repo.UpdateEjecutivoAsync(nuevoEjecutivo);

            if (result.success)
            {
                MessageBox.Show("Ejecutivo actualizado correctramente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                ReadEjecutivo();
                LimpiarFormulario();
            }
            else
            {
                MessageBox.Show("No se pudo actualizar: " + (result.errorMessage ?? "Error desconocido"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ejecutivoSeleccionado = (EjecutivoM)dataGridEjecutivo.SelectedItem;
    
            if (ejecutivoSeleccionado != null)
            {
                
                CargarDatosEjecutivoEnFormulario(ejecutivoSeleccionado);
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


        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string idBuscado = txtSearch.Text;

            if (string.IsNullOrWhiteSpace(idBuscado))
            {
                MessageBox.Show("Por favor, ingrese un ID para buscar.", "Entrada Requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EjecutivoRep repository = new EjecutivoRep();
            EjecutivoM? ejecutivoEncontrado = repository.GetEjecutivo(idBuscado);

            if (ejecutivoEncontrado != null)
            {

                CargarDatosEjecutivoEnFormulario(ejecutivoEncontrado);

                MessageBox.Show($"Ejecutivo encontrado: {ejecutivoEncontrado.nombre} {ejecutivoEncontrado.aPaterno}", "Búsqueda Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"No se encontró ningún ejecutivo con el ID: {idBuscado}", "No Encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
