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
    /// <summary>
    /// Interaction logic for UserControlBuscarPropuesta.xaml
    /// </summary>
    public partial class UserControlBuscarPropuesta : System.Windows.Controls.UserControl
    {
        private PropuestaRep propuestaRep = new PropuestaRep();
        private MonedaRep monedaRep = new MonedaRep();
        private ClienteRep clienteRep = new ClienteRep();
        private CompaniaRep companiaRep = new CompaniaRep();
        private RamoRep ramoRep = new RamoRep();
        private SocioRep socioRep = new SocioRep();
        private GestorRep gestorRep= new GestorRep();

        private List<Moneda> monedas;
        private List<Cliente> clientes;
        private List<Compania> companias;
        private List<Ramo> ramos;
        private List<Socio> socios;
        private List<Gestor> gestores;
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
            var propuestaBuscada = new Propuesta();
            propuestaBuscada = propuestaRep.GetPropuesta(int.Parse(txtNroPoliza.Text));

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

            cbContratante.SelectedItem = clientes.Find(cliente => cliente.ID== propuestaBuscada.IDCliente);

            var clienteBuscado = (Cliente)cbContratante.SelectedItem;

            cbCompania.SelectedItem = companias.Find(compania => compania.ID == propuestaBuscada.IDCompania);
            cbRamo.SelectedItem = ramos.Find(ramo => ramo.ID == propuestaBuscada.IDRamo);
            txtRutAsegurado.Text = clienteBuscado.ID;
            cbSocio.SelectedItem = socios.Find(socio => socio.ID == propuestaBuscada.IDSocio);
            cbGestor.SelectedItem = gestores.Find(gestor => gestor.ID == propuestaBuscada.IDGestor);

            dpDesde.IsEnabled = true;
            cbMoneda.IsEnabled = true;
            cbTipoPoliza.IsEnabled = true;
            cbContratante.IsEnabled = true;
            cbCompania.IsEnabled = true;
            cbRamo.IsEnabled = true;
            dpHasta.IsEnabled = true;
            txtRutAsegurado.IsEnabled = true;
            txtPatente.IsEnabled = true;
            cbSocio.IsEnabled = true;
            cbGestor.IsEnabled = true;
        }
    }
}
