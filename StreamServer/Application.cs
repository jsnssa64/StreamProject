using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamServer
{
    public class Application
    {
        public Application(IFileStorageBase test)
        {
            Console.WriteLine(test.FSBuild.ToString());
            Console.ReadLine();
            //  Inject Services Here
        }
        //  This is the new main of your application
        public void Run()
        {
            //  Code Here
            Console.ReadLine();
        }
    }
}
