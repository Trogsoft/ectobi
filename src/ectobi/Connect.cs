using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.CommandLine;

namespace ectobi
{
    [Verb("connect")]
    public class Connect : Verb
    {

        [Operation(isDefault: true)]
        public int ConnectToInstance(string hostname)
        {
            InstanceManager.SetInstance(hostname);
            Console.WriteLine("Instance updated.");
            return 0;
        }

    }
}
