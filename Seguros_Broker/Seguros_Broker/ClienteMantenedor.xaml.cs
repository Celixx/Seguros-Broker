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
    /// Interaction logic for ClienteMantenedor.xaml
    /// </summary>
    public partial class ClienteMantenedor : Window
    {
        private List<Holding> holdings;
        private HoldingRep holdingRep = new HoldingRep();

        private List<EjecutivoM> ejecutivos;
        private EjecutivoRep ejecutivoRep = new EjecutivoRep(); 

        public ClienteMantenedor()
        {
            InitializeComponent();

            this.holdings = holdingRep.GetHoldings();
            try
            {
                this.ejecutivos = ejecutivoRep.GetEjecutivos();
            }
            catch
            {

                this.ejecutivos = new List<EjecutivoM>();
            }

            ReadCliente();
            ReadHolding();
            ReadEjecutivo();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Seguro que quiere salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) this.Close();
        }

        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones básicas
            var errores = new List<string>();

            if (cbTipoIdentificacion.SelectedItem == null ||
                ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString().ToUpper().Contains("SELECCIONE"))
                errores.Add("Tipo identificación (obligatorio).");

            if (string.IsNullOrWhiteSpace(txtIdentificacion.Text))
                errores.Add("Identificación (obligatorio).");

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                errores.Add("Nombre (obligatorio).");

            if (cbHolding.SelectedItem == null)
                errores.Add("Holding (obligatorio).");

            if (cbEjecutivo.SelectedItem == null)
                errores.Add("Director Comercial (obligatorio).");

            if (errores.Any())
            {
                MessageBox.Show("Corrija los siguientes errores:\n- " + string.Join("\n- ", errores), "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedHolding = cbHolding.SelectedItem as Holding;
            var selectedEjecutivo = cbEjecutivo.SelectedItem as EjecutivoM;

            var nuevoCliente = new Cliente
            {
                TipoIdentificacion = ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString(),
                ID = txtIdentificacion.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                ApellidoPaterno = txtAPaterno.Text?.Trim() ?? "",
                ApellidoMaterno = txtAMaterno.Text?.Trim() ?? "",
                HoldingID = selectedHolding != null ? selectedHolding.ID : 0,
                HoldingNombre = selectedHolding != null ? selectedHolding.Nombre : "",
                EjecutivoID = selectedEjecutivo != null ? selectedEjecutivo.ID : "",
                EjecutivoNombre = selectedEjecutivo != null ? selectedEjecutivo.nombre : "",
                Fonos = txtFono.Text?.Trim() ?? "",
                PaginaWeb = txtPagina.Text?.Trim() ?? "",
                NombreCorto = txtCorto.Text?.Trim() ?? "",
                Referencia = txtReferencia.Text?.Trim() ?? "",

                Particular_Pais = txtPaisParticular.Text?.Trim() ?? "DESCONOCIDO",
                Particular_Region = txtRegionParticular.Text?.Trim() ?? "",
                Particular_Ciudad = txtCiudadParticular.Text?.Trim() ?? "",
                Particular_Comuna = txtComunaParticular.Text?.Trim() ?? "",
                Particular_Direccion = txtDireccionParticular.Text?.Trim() ?? "",

                Comercial_Pais = txtPaisComercial.Text?.Trim() ?? "DESCONOCIDO",
                Comercial_Region = txtRegionComercial.Text?.Trim() ?? "",
                Comercial_Ciudad = txtCiudadComercial.Text?.Trim() ?? "",
                Comercial_Comuna = txtComunaComercial.Text?.Trim() ?? "",
                Comercial_Direccion = txtDireccionComercial.Text?.Trim() ?? ""
            };

            var repo = new ClienteRep();
            var result = await repo.CreateClienteAsync(nuevoCliente);

            if (result.success)
            {
                MessageBox.Show("Cliente guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                ReadCliente();
                LimpiarFormulario();
            }
            else
            {
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

            ClienteRep repository = new ClienteRep();
            Cliente? clienteEncontrado = repository.GetCliente(idBuscado);

            if (clienteEncontrado != null)
            {
                CargarDatosClienteEnFormulario(clienteEncontrado);
                MessageBox.Show($"Cliente encontrado: {clienteEncontrado.Nombre} {clienteEncontrado.ApellidoPaterno}", "Búsqueda Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"No se encontró ningún cliente con el ID: {idBuscado}", "No Encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CargarDatosClienteEnFormulario(Cliente cliente)
        {
            if (cliente == null) return;

            cbTipoIdentificacion.Text = cliente.TipoIdentificacion;
            txtIdentificacion.Text = cliente.ID;
            txtNombre.Text = cliente.Nombre;
            txtAPaterno.Text = cliente.ApellidoPaterno;
            txtAMaterno.Text = cliente.ApellidoMaterno;

            cbHolding.SelectedItem = holdings.Find(h => h.ID == cliente.HoldingID);
            cbEjecutivo.SelectedItem = ejecutivos.Find(e => e.ID == cliente.EjecutivoID);

            txtFono.Text = cliente.Fonos;
            txtPagina.Text = cliente.PaginaWeb;
            txtCorto.Text = cliente.NombreCorto;
            txtReferencia.Text = cliente.Referencia;

            txtPaisParticular.Text = cliente.Particular_Pais;
            txtCiudadParticular.Text = cliente.Particular_Ciudad;
            txtRegionParticular.Text = cliente.Particular_Region;
            txtComunaParticular.Text = cliente.Particular_Comuna;
            txtDireccionParticular.Text = cliente.Particular_Direccion;

            txtPaisComercial.Text = cliente.Comercial_Pais;
            txtCiudadComercial.Text = cliente.Comercial_Ciudad;
            txtRegionComercial.Text = cliente.Comercial_Region;
            txtComunaComercial.Text = cliente.Comercial_Comuna;
            txtDireccionComercial.Text = cliente.Comercial_Direccion;
        }

        private void LimpiarFormulario()
        {
            cbTipoIdentificacion.SelectedIndex = 0;
            txtIdentificacion.Clear();
            txtNombre.Clear();
            txtAPaterno.Clear();
            txtAMaterno.Clear();
            cbHolding.SelectedIndex = -1;
            cbEjecutivo.SelectedIndex = -1;
            txtFono.Clear();
            txtPagina.Clear();
            txtCorto.Clear();
            txtReferencia.Clear();

            txtPaisParticular.Text = "DESCONOCIDO";
            txtCiudadParticular.Clear();
            txtRegionParticular.Clear();
            txtComunaParticular.Clear();
            txtDireccionParticular.Clear();

            txtPaisComercial.Text = "DESCONOCIDO";
            txtCiudadComercial.Clear();
            txtRegionComercial.Clear();
            txtComunaComercial.Clear();
            txtDireccionComercial.Clear();
        }

        private void ReadCliente()
        {
            var repo = new ClienteRep();
            var clientes = repo.GetClientes();

            cbHolding.ItemsSource = holdings;
            cbEjecutivo.ItemsSource = ejecutivos;

            this.dataGridCliente.ItemsSource = clientes;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var clienteSeleccionado = (Cliente)dataGridCliente.SelectedItem;
            if (clienteSeleccionado != null)
            {
                CargarDatosClienteEnFormulario(clienteSeleccionado);
            }
        }

        // Holdings tab — operaciones sobre holdings (Crear/Actualizar/Buscar) adaptadas del ejemplo Grupo
        private void ReadHolding()
        {
            this.dataGridHolding.ItemsSource = holdings;
            cbHolding.ItemsSource = holdings;
        }

        private async void btnGuardar_Holding_Click(object sender, RoutedEventArgs e)
        {
            var errores = new List<string>();
            if (string.IsNullOrWhiteSpace(txtNombreHolding.Text))
                errores.Add("Nombre (obligatorio).");

            if (errores.Any())
            {
                MessageBox.Show("Corrija los siguientes errores:\n- " + string.Join("\n- ", errores), "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nuevoHolding = new Holding
            {
                ID = 0,
                Nombre = txtNombreHolding.Text.Trim()
            };

            var repo = new HoldingRep();
            var result = await repo.CreateHoldingAsync(nuevoHolding);

            if (result.success)
            {
                MessageBox.Show("Holding guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                holdings = holdingRep.GetHoldings();
                ReadHolding();
                Limpiar_Holder_Form();
                ReadCliente();
            }
            else
            {
                MessageBox.Show("No se pudo guardar: " + (result.errorMessage ?? "Error desconocido"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Limpiar_Holder_Form()
        {
            txtNombreHolding.Clear();
        }

        private void btnBuscar_Grupo_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtSearch_ClienteHolding.Text, out int idBuscado))
            {
                MessageBox.Show("Ingrese un ID numérico para buscar.", "Entrada Requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var repo = new HoldingRep();
            var holdingEncontrado = repo.GetHolding(idBuscado);
            if (holdingEncontrado != null)
            {
                txtNombreHolding.Text = holdingEncontrado.Nombre;
                MessageBox.Show($"Holding encontrado: {holdingEncontrado.Nombre}", "Búsqueda Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"No se encontró ningún holding con el ID: {idBuscado}", "No Encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void btnActualizar_Holding_Click(object sender, RoutedEventArgs e)
        {
            var errores = new List<string>();
            if (string.IsNullOrWhiteSpace(txtNombreHolding.Text))
                errores.Add("Nombre (obligatorio).");

            if (errores.Any())
            {
                MessageBox.Show("Corrija los siguientes errores:\n- " + string.Join("\n- ", errores), "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!(dataGridHolding.SelectedItem is Holding holdingSeleccionado))
            {
                MessageBox.Show("Seleccione un holding para actualizar.", "Selección Requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nuevoHolding = new Holding
            {
                ID = holdingSeleccionado.ID,
                Nombre = txtNombreHolding.Text.Trim()
            };

            var result = await holdingRep.UpdateHoldingAsync(nuevoHolding);
            if (result.success)
            {
                MessageBox.Show("Holding actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                holdings = holdingRep.GetHoldings();
                ReadHolding();
                Limpiar_Holder_Form();
            }
            else
            {
                MessageBox.Show("No se pudo actualizar: " + (result.errorMessage ?? "Error desconocido"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadEjecutivo()
        {
            // Si EjecutivoRep no existe o su GetEjecutivos falla, la lista estará vacía (manejado en ctor)
            this.cbEjecutivo.ItemsSource = ejecutivos;
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnLimpiar_Holder_Click(object sender, RoutedEventArgs e)
        {
            Limpiar_Holder_Form();
        }

        private async void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones (mismas que en Guardar)
            var errores = new List<string>();

            if (cbTipoIdentificacion.SelectedItem == null ||
                ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString().ToUpper().Contains("SELECCIONE"))
                errores.Add("Tipo identificación (obligatorio).");

            if (string.IsNullOrWhiteSpace(txtIdentificacion.Text))
                errores.Add("Identificación (obligatorio).");

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                errores.Add("Nombre (obligatorio).");

            if (cbHolding.SelectedItem == null)
                errores.Add("Holding (obligatorio).");

            if (cbEjecutivo.SelectedItem == null)
                errores.Add("Director Comercial (obligatorio).");

            if (errores.Any())
            {
                MessageBox.Show("Corrija los siguientes errores:\n- " + string.Join("\n- ", errores), "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedHolding = cbHolding.SelectedItem as Holding;
            var selectedEjecutivo = cbEjecutivo.SelectedItem as EjecutivoM;

            var clienteActualizado = new Cliente
            {
                TipoIdentificacion = ((ComboBoxItem)cbTipoIdentificacion.SelectedItem).Content.ToString(),
                ID = txtIdentificacion.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                ApellidoPaterno = txtAPaterno.Text?.Trim() ?? "",
                ApellidoMaterno = txtAMaterno.Text?.Trim() ?? "",
                HoldingID = selectedHolding != null ? selectedHolding.ID : 0,
                HoldingNombre = selectedHolding != null ? selectedHolding.Nombre : "",
                EjecutivoID = selectedEjecutivo != null ? selectedEjecutivo.ID : "",
                EjecutivoNombre = selectedEjecutivo != null ? selectedEjecutivo.nombre : "",
                Fonos = txtFono.Text?.Trim() ?? "",
                PaginaWeb = txtPagina.Text?.Trim() ?? "",
                NombreCorto = txtCorto.Text?.Trim() ?? "",
                Referencia = txtReferencia.Text?.Trim() ?? "",

                Particular_Pais = txtPaisParticular.Text?.Trim() ?? "DESCONOCIDO",
                Particular_Region = txtRegionParticular.Text?.Trim() ?? "",
                Particular_Ciudad = txtCiudadParticular.Text?.Trim() ?? "",
                Particular_Comuna = txtComunaParticular.Text?.Trim() ?? "",
                Particular_Direccion = txtDireccionParticular.Text?.Trim() ?? "",

                Comercial_Pais = txtPaisComercial.Text?.Trim() ?? "DESCONOCIDO",
                Comercial_Region = txtRegionComercial.Text?.Trim() ?? "",
                Comercial_Ciudad = txtCiudadComercial.Text?.Trim() ?? "",
                Comercial_Comuna = txtComunaComercial.Text?.Trim() ?? "",
                Comercial_Direccion = txtDireccionComercial.Text?.Trim() ?? ""
            };

            var repo = new ClienteRep();
            var result = await repo.UpdateClienteAsync(clienteActualizado);

            if (result.success)
            {
                MessageBox.Show("Cliente actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                ReadCliente();
                LimpiarFormulario();
            }
            else
            {
                MessageBox.Show("No se pudo actualizar: " + (result.errorMessage ?? "Error desconocido"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DataGrid_SelectionChanged_Holding(object sender, SelectionChangedEventArgs e)
        {
            // Obtener el holding seleccionado del DataGrid (si hay uno)
            if (dataGridHolding.SelectedItem is Holding holdingSeleccionado)
            {
                // Cargar datos del holding en el formulario de holdings
                txtNombreHolding.Text = holdingSeleccionado.Nombre;

                // También sincronizar el ComboBox de holdings para que refleje la selección actual
                if (holdings != null && holdings.Count > 0)
                {
                    var encontrado = holdings.Find(h => h.ID == holdingSeleccionado.ID);
                    if (encontrado != null)
                    {
                        cbHolding.SelectedItem = encontrado;
                    }
                }
            }
            else
            {
                // Si no hay selección, limpiar el formulario de holdings (opcional)
                txtNombreHolding.Clear();
            }
        }

        private void btnCancelar_Holding_Click(object sender, RoutedEventArgs e)
        {
                    MessageBoxResult result = MessageBox.Show( "¿Seguro que quiere salir?","Confirmación", MessageBoxButton.YesNo,MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
            this.Close();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
