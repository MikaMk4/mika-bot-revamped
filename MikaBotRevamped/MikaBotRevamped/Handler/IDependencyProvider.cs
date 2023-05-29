using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Handler
{
    public interface IDependencyProvider
    {
        TDependency GetDependency<TDependency>();
    }
}
