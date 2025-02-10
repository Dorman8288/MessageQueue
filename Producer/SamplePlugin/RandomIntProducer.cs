using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePlugin
{
   public class RandomIntProducer : IProducer<int>
    {
        public int Produce()
        {
            Random random = new Random();
            return random.Next();
        }
    }
}
