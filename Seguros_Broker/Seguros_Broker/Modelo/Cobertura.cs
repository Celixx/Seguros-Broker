using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Seguros_Broker.Modelo
{
    // 1. Implementar la interfaz
    public class Cobertura : INotifyPropertyChanged
    {
        // 2. Evento requerido por la interfaz
        public event PropertyChangedEventHandler PropertyChanged;

        // 3. Método pomocniczy do wywoływania zdarzenia
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // --- Propiedades que notifican cambios ---

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(); // <-- Notificar a la UI
                }
            }
        }

        // (Opcional, pero buena práctica)
        // Hacemos que todas las propiedades que se ven en la UI notifiquen
        private string _codigo;
        public string codigo
        {
            get { return _codigo; }
            set
            {
                if (_codigo != value)
                {
                    _codigo = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _nombre;
        public string nombre
        {
            get { return _nombre; }
            set
            {
                if (_nombre != value)
                {
                    _nombre = value;
                    OnPropertyChanged();
                }
            }
        }

        // --- Resto de propiedades (pueden ser simples) ---
        public string afectaExtenta { get; set; }
        public string sumaMonto { get; set; }
        public int monto { get; set; }
        public int prima { get; set; }
    }
}