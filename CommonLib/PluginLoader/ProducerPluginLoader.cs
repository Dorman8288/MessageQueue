using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.PluginLoader
{
    public class ProducerPluginLoader : PluginLoaderBase
    {
        public ProducerPluginLoader(ILogger logger) : base(logger)
        {
        }

        public override bool IsEligableType(Type type) => type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IProducer<>));
    }
}
