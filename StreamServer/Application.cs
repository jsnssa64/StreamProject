using Microsoft.Extensions.Configuration;
using StreamServer.FileStorage.FSModel;
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
        public Application(IFileStorageModel test)
        {
            Console.WriteLine(test.ToString());
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

    /*  
    * public void GetFile(List<IConfigurationSection> CredentialConfig)
        {
            switch (storageType)
            {
                case ExternalStorageType.azure:
                    string AzureSecretType = FileStorageUtilities.FindKeyValuePair(CredentialTypeKeyName, CredentialConfig).Value.ToLower();
                    FileStorageUtilities.AzureFileServiceManager(AzureSecretType, CredentialConfig);
                    break;
                case ExternalStorageType.aws:
                    FileStorageUtilities.AWSFileServiceManager(CredentialConfig);
                    break;
                default:
                    throw new Exception("Credentials Not Found");
            }

            throw new NotImplementedException();
        }*/
}
