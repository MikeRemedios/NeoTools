using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoFunctions;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Neoquest_II_AP
{
    
    class Program
    {
        static void Main(string[] args)
        {
            NQLoop game = new NQLoop();
            game.StartGame();

            Console.ReadLine();
        }
    }
}
