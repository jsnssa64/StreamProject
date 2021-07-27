using Microsoft.Extensions.Configuration;
using SecretServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SecretServices.CredentialsModel;

namespace StreamServer.Utility
{
    public static class FileStorageUtilities
    {
        public static IConfigurationSection FindKeyValuePair(string Key, List<IConfigurationSection> ConfigList)
        {
            IConfigurationSection ConfigContent = ConfigList.Find(x => x.Key.ToLower() == Key.ToLower());

            if (ConfigContent == null)
                throw new Exception("Empty Configuration");

            return ConfigContent;
        }

        public static void AzureFileServiceManager(string type, List<IConfigurationSection> CredDetails)
        {
            throw new NotImplementedException();
        }

        public static void AWSFileServiceManager(List<IConfigurationSection> CredDetails)
        {
            throw new NotImplementedException();
        }
    }
}
