using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    internal interface IDependencyProvider
    {
        TDependency GetDependency<TDependency>();
    }
}
