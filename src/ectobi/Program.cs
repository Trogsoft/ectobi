using Trogsoft.CommandLine;

namespace ectobi
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Parser p = new Parser();
            p.Run(args);

        }
    }
}