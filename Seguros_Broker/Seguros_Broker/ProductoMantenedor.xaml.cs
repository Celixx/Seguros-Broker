using Seguros_Broker.Modelo;
using Seguros_Broker.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Seguros_Broker
{
    /// <summary>
    /// Interaction logic for ProductoMantenedor.xaml
    /// </summary>
    public partial class ProductoMantenedor : Window
    {
        private List<Ramo> ramos;
        private RamoRep ramoRep = new RamoRep();

        private List<Compania> companias;
        private CompaniaRep companiaRep = new CompaniaRep();

        private List<Producto> productos;
        private ProductoRep productoRep = new ProductoRep();

        private CoberturaRep coberturaRep = new CoberturaRep();

        private int selectedProductId = 0;

        public ProductoMantenedor()
        {
            InitializeComponent();

            this.ramos = ramoRep.GetRamos();
            this.companias = companiaRep.GetCompanias();
            ReadProductos();
            ReadRamos();
            ReadCompanias();
        }

        private void ReadRamos()
        {
            cbRamo.ItemsSource = ramos;
        }

        private void ReadCompanias()
        {
            cbCompania.ItemsSource = companias;
        }

        private void ReadProductos()
        {
            productos = productoRep.GetProductos();
            dataGridCoberturas.ItemsSource = productos;
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            var errores = new List<string>();
            if (string.IsNullOrWhiteSpace(txtCodigo.Text)) errores.Add("Nombre (obligatorio).");
            if (cbRamo.SelectedItem == null) errores.Add("Ramo (obligatorio).");
            if (cbCompania.SelectedItem == null) errores.Add("Compañía (obligatorio).");
            if (errores.Any())
            {
                MessageBox.Show("Corrija los siguientes errores:\n- " + string.Join("\n- ", errores), "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedRamo = cbRamo.SelectedItem as Ramo;
            var selectedCompania = cbCompania.SelectedItem as Compania;

            var nuevoProducto = new Producto
            {
                nombre = txtCodigo.Text.Trim(),
                ramoID = selectedRamo != null ? selectedRamo.ID : 0,
                companiaID = selectedCompania != null ? selectedCompania.ID : ""
            };

            var (success, errorMessage, insertedId) = await productoRep.CreateProductoAsync(nuevoProducto);
            if (success)
            {
                MessageBox.Show("Producto guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // refrescar lista de productos 
                ReadProductos();

                // seleccionar el producto recién insertado en el dataGrid
                selectedProductId = insertedId;


                // habilitar botón de agregar cobertura
                btnCobertura.IsEnabled = true;

                LimpiarFormulario();
            }
            else
            {
                MessageBox.Show("No se pudo guardar: " + (errorMessage ?? "Error desconocido"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimpiarFormulario()
        {
            txtCodigo.Clear();
            cbRamo.SelectedIndex = -1;
            cbCompania.SelectedIndex = -1;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Seguro que quiere salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) this.Close();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridCoberturas.SelectedItem is Producto seleccionado)
            {
                CargarProductoEnFormulario(seleccionado);
            }
        }


        private void CargarProductoEnFormulario(Producto producto)
        {
            if (producto == null) return;
            txtCodigo.Text = producto.nombre;
            cbRamo.SelectedItem = ramos?.Find(r => r.ID == producto.ramoID);
            cbCompania.SelectedItem = companias?.Find(c => c.ID == producto.companiaID);
        }

        private void btnCobertura_Click(object sender, RoutedEventArgs e)
        {
            if (selectedProductId <= 0)
            {
                MessageBox.Show("Seleccione primero un producto.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var ventana = new ProductoCobertura(selectedProductId);
            ventana.Owner = this;
            ventana.ShowDialog();

            //Recargar el dataGrid con los datos nuevos
            var rows = productoRep.GetProductoCoberturas(selectedProductId);
            dataGridCoberturas.ItemsSource = rows;
        }
    }
}
