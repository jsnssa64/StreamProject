using SecretServices.CredentialsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamServer.FileStorage.FSModel
{

    public class FilePath
    {
        public string filePath { get; set; }

        public FilePath(string FilePath)
        {
            filePath = FilePath;
        }
    }

    public class CurrentDirectoryFilePath : FilePath
    {

        public CurrentDirectoryFilePath(string FilePath) : base(FilePath)
        {
            // TODO
            // currentDirectory = [Get Projects Current Directory from settings] + "wwwroot/FileStore"
            filePath = AppDomain.CurrentDomain.BaseDirectory + FilePath;
            Console.WriteLine("AppDomain.CurrentDomain.BaseDirectory:");
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
