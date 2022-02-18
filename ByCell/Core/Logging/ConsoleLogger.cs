using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Logging
{
    public class ConsoleLogger : ICustomLogger
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}
