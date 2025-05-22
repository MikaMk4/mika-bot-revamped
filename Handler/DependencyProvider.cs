using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Handler
{
    public class DependencyProvider : IDependencyProvider
    {
        private Dictionary<Type, object> dependencies = new Dictionary<Type, object>();

        public void RegisterDependency<TDependency>(TDependency dependency) where TDependency : class
        {
            dependencies[typeof(TDependency)] = dependency;
        }

        public TDependency GetDependency<TDependency>()
        {
            if (dependencies.TryGetValue(typeof(TDependency), out var dependency))
            {
                return (TDependency)dependency;
            }

            throw new InvalidOperationException($"Dependency {typeof(TDependency).Name} is not registered.");
        }
    }
}
