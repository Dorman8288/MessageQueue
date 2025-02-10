using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Utils
    {
        public static string GenerateRandomString(int length)
        {
            Random random = new Random();
            return string.Join("", Enumerable.Range(0, length).Select(i => Convert.ToChar(random.Next(Convert.ToInt32('a'), Convert.ToInt32('z')))).ToArray());
        }
    }
}
