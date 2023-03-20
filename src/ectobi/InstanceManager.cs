using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.Ectobi.Client;

namespace ectobi
{
    internal static class InstanceManager
    {
        static string path;
        private static string idat;
        private static EctoClient client;
        private static string instance;

        static InstanceManager()
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path = Path.Combine(appdata, "Ectobi", "Cmd");
            Directory.CreateDirectory(path);
            idat = Path.Combine(path, "instance.dat");
        }

        public static void SetInstance(string hostname)
        {
            instance = hostname;
            CreateClient();
            File.WriteAllText(idat, hostname);
        }

        public static string Instance
        {
            get
            {
                if (File.Exists(idat))
                {
                    return File.ReadAllText(idat);
                }
                else
                {
                    return "cloud.ectobi.com";
                }
            }
        }

        public static EctoClient Client
        {
            get
            {
                if (client == null)
                {
                    CreateClient();
                }
                return client;
            }
        }

        private static void CreateClient()
        {
            var http = new HttpClient
            {
                BaseAddress = new Uri($"https://{Instance}")
            };
            client = new EctoClient(http);
        }
    }
}
