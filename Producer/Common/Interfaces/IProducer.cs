using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IProducer
    {
        string Produce();
    }

    public interface IProducer<T>
    {
        T Produce();
    }
}
