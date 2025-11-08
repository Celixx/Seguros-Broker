using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguros_Broker.Repositorio
{
    public abstract class Repositorio
    {
        protected readonly string connectionString = Configuration.Get().connectionString;
    }
}
