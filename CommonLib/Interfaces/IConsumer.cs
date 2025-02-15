using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Interfaces
{
    public interface IConsumer<in T>
    {
        void Consume (T item);
    }
}
