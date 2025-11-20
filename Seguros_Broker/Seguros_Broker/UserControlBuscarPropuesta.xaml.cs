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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Seguros_Broker
{
    
    public partial class UserControlBuscarPropuesta : System.Windows.Controls.UserControl
    {
        private PropuestaRep propuestaRep = new PropuestaRep();
        private MonedaRep monedaRep = new MonedaRep();
        private ClienteRep clienteRep = new ClienteRep();
        private CompaniaRep companiaRep = new CompaniaRep();
        private RamoRep ramoRep = new RamoRep();
        private SocioRep socioRep = new SocioRep();
        private GestorRep gestorRep= new GestorRep();
        private PagosPropuestaRep pagosRep = new PagosPropuestaRep();
        private ItemRep itemRep = new ItemRep();

        private List<Moneda> monedas;
        private List<Cliente> clientes;
        private List<Compania> companias;
        private List<Ramo> ramos;
        private List<Socio> socios;
        private List<Gestor> gestores;
        private List<Item> items;
        public UserControlBuscarPropuesta()
        {
            InitializeComponent();

            var propuestas = propuestaRep.GetPropuestas();
            dataGridPropuestas.ItemsSource = propuestas;

            this.monedas = monedaRep.GetMonedas();
            cbMoneda.ItemsSource = monedas;

            this.clientes = clienteRep.GetClientes();
            cbContratante.ItemsSource = clientes;

            this.companias = companiaRep.GetCompanias();
            cbCompania.ItemsSource = companias;

            this.ramos = ramoRep.GetRamos();
            cbRamo.ItemsSource = ramos;

            this.socios = socioRep.GetSocios();
            cbSocio.ItemsSource = socios;

            this.gestores = gestorRep.GetGestores();
            cbGestor.ItemsSource = gestores;

            
        }

        private void btnBuscarNumeroPoliza_Click(object sender, RoutedEventArgs e)
        {
            if (txtNroPoliza.Text == "")
            {
                System.Windows.MessageBox.Show("Por favor ingresar un número de póliza antes de buscar.", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var propuestaBuscada = new Propuesta();
            try
            {
                propuestaBuscada = propuestaRep.GetPropuesta(int.Parse(txtNroPoliza.Text));
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("El número de póliza ingresado no es válido o no se encuentra registrado", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            
            dpDesde.SelectedDate = propuestaBuscada.FechaVigenciaDesde;
            dpHasta.SelectedDate = propuestaBuscada.FechaVigenciaHasta;
            cbMoneda.SelectedItem = (Moneda)(monedas.Find(moneda => moneda.monedaId == propuestaBuscada.IDMoneda));

            if (propuestaBuscada.TipoPoliza == "Póliza Convencional")
            {
                cbTipoPoliza.SelectedIndex = 0;
            }
            else
            {
                cbTipoPoliza.SelectedIndex = 1;
            }

            cbContratante.SelectedItem = clientes.Find(cliente => cliente.ID == propuestaBuscada.IDCliente);
            var clienteBuscado = (Cliente)cbContratante.SelectedItem;

            cbCompania.SelectedItem = companias.Find(compania => compania.ID == propuestaBuscada.IDCompania);
            cbRamo.SelectedItem = ramos.Find(ramo => ramo.ID == propuestaBuscada.IDRamo);
            txtRutAsegurado.Text = clienteBuscado.ID;
            cbSocio.SelectedItem = socios.Find(socio => socio.ID == propuestaBuscada.IDSocio);
            cbGestor.SelectedItem = gestores.Find(gestor => gestor.ID == propuestaBuscada.IDGestor);

            cbContratante.IsEnabled = true;
            cbCompania.IsEnabled = true;

            btnLimpiar.IsEnabled = true;
            btnGuardar.IsEnabled = true;

            
            try
            {
                
                var pagos = pagosRep.GetPagosByPropuestaID(propuestaBuscada.ID);

                if (pagos != null && pagos.Count > 0)
                {
                    
                    var lines = new List<string>();
                    foreach (var p in pagos)
                    {
                        
                        lines.Add($"Cuota {p.CuotaNro}: {p.Monto:N2}");
                    }

                 
                    txtPlanPago.Text = string.Join(Environment.NewLine, lines);

                }

                else
                {
                    txtPlanPago.Text = ""; 
                }
            }
            catch (Exception ex)
            {
             
                System.Windows.MessageBox.Show("No se pudo cargar el Plan de Pago: " + ex.Message, "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            try
            {
                
                var items = itemRep.GetItemsByRut(clienteBuscado.ID);

                if (items != null && items.Count > 0)
                {
                   

                    var listaPatentes = items
                                        .Select(i => i.Patente)
                                        .Where(p => !string.IsNullOrWhiteSpace(p))
                                        .Distinct();

                    txtPatente.Text = string.Join(", ", listaPatentes);
                }
                else
                {
                    txtPatente.Text = "S/N"; 
                }
            }
            catch (Exception ex)
            {
                
                txtPatente.Text = "";
                System.Windows.MessageBox.Show("No se pudo cargar la Patente: " + ex.Message, "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {

            var companiaSelected = (Compania)cbCompania.SelectedItem;
            var clienteSelected = (Cliente)cbContratante.SelectedItem;
            var propuestaActual = propuestaRep.GetPropuesta(int.Parse(txtNroPoliza.Text));

            var propuestaUpdate = new Propuesta();
            propuestaUpdate.IDCompania = companiaSelected.ID;
            propuestaUpdate.IDCliente = clienteSelected.ID;
            propuestaUpdate.ID = propuestaActual.ID;

            var result = await propuestaRep.UpdatePropuestaAsync(propuestaUpdate);

            if (result.success)
            {
                System.Windows.MessageBox.Show("Propuesta actualizada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);



               
                ReadPropuesta();

              
                LimpiarFormulario();
            }
            else
            {
                
                System.Windows.MessageBox.Show("No se pudo actualizar: " + (result.errorMessage ?? "Error desconocido"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LimpiarFormulario()
        {
            dpDesde.SelectedDate = null;
            cbMoneda.SelectedIndex = -1;
            cbTipoPoliza.SelectedIndex = -1;
            cbContratante.SelectedIndex = -1;
            cbCompania.SelectedIndex = -1;
            cbRamo.SelectedIndex = -1;
            dpHasta.SelectedDate = null;
            txtNroPoliza.Text = null;
            txtRutAsegurado.Text = null;
            txtPatente.Text = null;
            txtEstado.Text = null;
            cbSocio.SelectedIndex = -1;
            cbGestor.SelectedIndex = -1;
            txtPlanPago.Text = null;
        }

        private void ReadPropuesta()
        {
            var propuestas = propuestaRep.GetPropuestas();

            this.dataGridPropuestas.ItemsSource = propuestas;
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
            btnLimpiar.IsEnabled = false;
            btnGuardar.IsEnabled = false;
        }

        private void dataGridPropuestas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var propuestaSeleccionada = (Propuesta)dataGridPropuestas.SelectedItem;

            if (propuestaSeleccionada != null)
            {
                dpDesde.SelectedDate = propuestaSeleccionada.FechaVigenciaDesde;
                dpHasta.SelectedDate = propuestaSeleccionada.FechaVigenciaHasta;
                cbMoneda.SelectedItem = (Moneda)(monedas.Find(moneda => moneda.monedaId == propuestaSeleccionada.IDMoneda));

                if (propuestaSeleccionada.TipoPoliza == "Póliza Convencional")
                {
                    cbTipoPoliza.SelectedIndex = 0;
                }
                else
                {
                    cbTipoPoliza.SelectedIndex = 1;
                }

                cbContratante.SelectedItem = clientes.Find(cliente => cliente.ID == propuestaSeleccionada.IDCliente);

                var clienteBuscado = (Cliente)cbContratante.SelectedItem;

                cbCompania.SelectedItem = companias.Find(compania => compania.ID == propuestaSeleccionada.IDCompania);
                cbRamo.SelectedItem = ramos.Find(ramo => ramo.ID == propuestaSeleccionada.IDRamo);
                txtRutAsegurado.Text = clienteBuscado.ID;
                cbSocio.SelectedItem = socios.Find(socio => socio.ID == propuestaSeleccionada.IDSocio);
                cbGestor.SelectedItem = gestores.Find(gestor => gestor.ID == propuestaSeleccionada.IDGestor);

                cbContratante.IsEnabled = true;
                cbCompania.IsEnabled = true;

                btnLimpiar.IsEnabled = true;
                btnGuardar.IsEnabled = true;
            }
        }
    }
}
