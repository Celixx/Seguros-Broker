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

            ReadGrupo();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Seguro que quiere salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
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

            //if (cbGrupo.SelectedItem == null ||
            //    ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString().ToUpper().Contains("SELECCIONE"))
            //    errores.Add("Tipo identificación (obligatorio).");

            if (errores.Any())
            {
                MessageBox.Show("Corrija los siguientes errores:\n- " + string.Join("\n- ", errores), "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Mapeo

            var selectedGrupo = cbGrupo.SelectedItem as Grupo;

            var nuevaCompania = new Compania{
                tipoID = ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString(),
                ID = txtIdentificacion.Text.Trim(),
                nombre = txtNombre.Text.Trim(),
                IDGrupo = selectedGrupo.ID,
                grupoNombre = selectedGrupo.Nombre,
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

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string idBuscado = txtSearch.Text;

            if (string.IsNullOrWhiteSpace(idBuscado))
            {
                MessageBox.Show("Por favor, ingrese un ID para buscar.", "Entrada Requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CompaniaRep repository = new CompaniaRep();
            Compania? companiaEncontrada = repository.GetCompania(idBuscado);

            if (companiaEncontrada != null)
            {

                CargarDatosCompaniaEnFormulario(companiaEncontrada);

                MessageBox.Show($"Compañía encontrada: {companiaEncontrada.nombre}", "Búsqueda Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"No se encontró ninguna compañía con el ID: {idBuscado}", "No Encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void CargarDatosCompaniaEnFormulario(Compania compania)
        {

            if (compania == null) return;

            txtIdentificacion.Text = compania.ID;
            txtNombre.Text = compania.nombre;
            cbGrupo.Text = compania.grupoNombre;
            txtFono.Text = compania.fono.ToString();
            txtPagina.Text = compania.paginaWeb;
            txtPais.Text = compania.pais;
            txtCiudad.Text = compania.ciudad;
            txtRegion.Text = compania.region;
            txtComuna.Text = compania.comuna;
            txtDireccion.Text = compania.direccion;
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
            var repoGrupos = new GrupoRep();

            var companias = repo.GetCompanias();
            var grupos = repoGrupos.GetGrupos();

            cbGrupo.ItemsSource = grupos;

            this.dataGridCompania.ItemsSource = companias;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnCancelar_Grupos_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Seguro que quiere salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private async void btnGuardar_Grupos_Click(object sender, RoutedEventArgs e)
        {
            var errores = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(txtNombre_Grupo.Text))
                errores.Add("Nombre (obligatorio).");

            if (errores.Any())
            {
                MessageBox.Show("Corrija los siguientes errores:\n- " + string.Join("\n- ", errores), "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Mapeo
            var nuevoGrupo = new Grupo
            {
                ID = 0,
                Nombre = txtNombre_Grupo.Text.Trim()
            };

            var repo = new GrupoRep();

            var result = await repo.CreateGrupoAsync(nuevoGrupo);

            if (result.success)
            {
                MessageBox.Show("Grupo guardado correctramente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                //Refrescar Grid
                ReadGrupo();

                //Limpiar Form
                LimpiarFormulario();
            }
            else
            {
                //Mostrar mensaje de error del repo
                MessageBox.Show("No se pudo guardar: " + (result.errorMessage ?? "Error desconocido"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadGrupo()
        {
            var repo = new GrupoRep();
            var grupos = repo.GetGrupos();

            this.dataGridGrupos.ItemsSource = grupos;
        }

        private void btnBuscar_Grupo_Click(object sender, RoutedEventArgs e)
        {
            string idBuscado = txtSearch_Grupo.Text;

            if (string.IsNullOrWhiteSpace(idBuscado))
            {
                MessageBox.Show("Por favor, ingrese un ID para buscar.", "Entrada Requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            GrupoRep repository = new GrupoRep();
            Grupo? grupoEncontrado = repository.GetGrupo(idBuscado);

            if (grupoEncontrado != null)
            {

                CargarDatosGrupoEnFormulario(grupoEncontrado);

                MessageBox.Show($"Grupo encontrado encontrada: {grupoEncontrado.Nombre}", "Búsqueda Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"No se encontró ninguna compañía con el ID: {idBuscado}", "No Encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CargarDatosGrupoEnFormulario(Grupo grupo)
        {

            if (grupo == null) return;

            txtNombre_Grupo.Text = grupo.Nombre;
        }

        private void LimpiarFormularioGrupo()
        {
            txtIdentificacion.Clear();
            txtNombre.Clear();
        }

        private void DataGrid_SelectionChanged_Grupo(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
