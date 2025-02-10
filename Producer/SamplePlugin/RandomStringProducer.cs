using Common;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePlugin
{
    public class RandomMessageProducer : IProducer
    {
        public int length { get; set; }
        public RandomMessageProducer(int length)
        {
            this.length = length;
        }
        public string Produce()
        {
            return Utils.GenerateRandomString(length);
        }
    }
}
