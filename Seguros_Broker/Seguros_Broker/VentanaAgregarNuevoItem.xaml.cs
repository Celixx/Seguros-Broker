using Seguros_Broker.Modelo;
using Seguros_Broker.Repositorio;
using System;
using System.Windows;

namespace Seguros_Broker
{
    public partial class VentanaAgregarNuevoItem : Window
    {
        private ItemRep itemRep = new ItemRep();
        private ClienteRep clienteRep = new ClienteRep();

        public VentanaAgregarNuevoItem(string rutClientePrellenado)
        {
            InitializeComponent();

 
            txtRutCliente.Text = rutClientePrellenado;
            dpDesde.SelectedDate = DateTime.Now;
            dpHasta.SelectedDate = DateTime.Now.AddYears(1);            
        }

        public VentanaAgregarNuevoItem()
        {
            InitializeComponent();

            //Valores Demo
            txtRutCliente.Text = "76.123.456-K";
            txtCarroceria.Text = "Sedán";
            txtMateriaAsegurada.Text = "Automóvil";
            txtValorComercial.Text = "15.000.000";
            txtTipo.Text = "Particular";
            txtAnno.Text = "2015";
            txtNumeroChasis.Text = "1";
            txtColor.Text = "Amarillo";
            txtMinutaItem.Text = "Minuta Auto Nuevo";
            txtChasis.Text = "CN1NJ13";
            txtPropietario.Text = "Rony Perez Carbone";
            txtMarca.Text = "Audi";
            txtModelo.Text = "R8";
            txtNumeroMotor.Text = "M-134664891";
            txtPatente.Text = "SV-15-31";
            txtUso.Text = "Particular";
            dpDesde.SelectedDate = DateTime.Now;
            dpHasta.SelectedDate = DateTime.Now.AddMonths(6);
        }

        private void BtaAceptarItem(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtRutCliente.Text))
            {
                MessageBox.Show("El RUT del Cliente es obligatorio.", "Falta información", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPatente.Text))
            {
                MessageBox.Show("La Patente es obligatoria.", "Falta información", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var buscarCliente = clienteRep.GetCliente(txtRutCliente.Text);
            if (buscarCliente == null)
            {
                MessageBox.Show("El cliente no existe.", "Falta información", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            




            Item nuevoItem = new Item
            {

                RutCliente = txtRutCliente.Text,

                Carroceria = txtCarroceria.Text,
                MateriaAsegurada = txtMateriaAsegurada.Text,
                ValorComercial = txtValorComercial.Text,
                Tipo = txtTipo.Text,
                Anno = txtAnno.Text,
                NumeroChasis = txtNumeroChasis.Text,
                Color = txtColor.Text,
                MinutaItem = txtMinutaItem.Text,
                Chasis = txtChasis.Text,
                Propietario = txtPropietario.Text,

               
                Modelo = txtModelo.Text,

                NumeroMotor = txtNumeroMotor.Text,
                Patente = txtPatente.Text,
                Uso = txtUso.Text,
                FechaDesde = dpDesde.SelectedDate,
                FechaHasta = dpHasta.SelectedDate
            };


            try
            {
                bool guardado = itemRep.AgregarItem(nuevoItem);

                if (guardado)
                {
                    MessageBox.Show("Item guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo guardar el item. Verifique los datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con base de datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}