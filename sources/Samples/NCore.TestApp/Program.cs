using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCore.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bootstrapper = new Bootstrapper())
            {
                bootstrapper.Start();

                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadLine();
            }
        }
    }
}
