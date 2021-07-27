using Microsoft.Extensions.Configuration;
using SecretServices;
using SecretServices.CredentialsModel;
using StreamServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamServer.FileStorage.SecretServiceRetrieval
{
    public interface IRetrieveSecretService
    {
        public IBaseSecrets GetSecretService(List<IConfigurationSection> CredentialsConfig);
    }

    public class AzureSecretServiceRetrieval : IRetrieveSecretService
    {
        public const string FilePathType = "Type";

        public IBaseSecrets GetSecretService(List<IConfigurationSection> CredentialsConfig)
        {
            string AzureCredentialsType = FileStorageUtilities.FindKeyValuePair(FilePathType, CredentialsConfig).Value.ToLower();
            return BuuldAzureSecretService(AzureCredentialsType, CredentialsConfig);
        }

        public static IBaseSecrets BuuldAzureSecretService(string type, List<IConfigurationSection> CredDetails)
        {
            string Uri = FileStorageUtilities.FindKeyValuePair("Uri", CredDetails).Value;
            string TenantId = FileStorageUtilities.FindKeyValuePair("TenantId", CredDetails).Value;
            string ClientId = FileStorageUtilities.FindKeyValuePair("ClientId", CredDetails).Value;

            switch (type.ToLower())
            {
                case "certificate":
                    string CertPath = FileStorageUtilities.FindKeyValuePair("CertificatePath", CredDetails).Value;
                    return new AzureKeyVaultCertificateService(new AzureCredentialsCertModel(Uri, TenantId, ClientId, CertPath));
                case "X509":
                    List<IConfigurationSection> CertificateConfig = FileStorageUtilities.FindKeyValuePair("Certificate", CredDetails).GetChildren().ToList();
                    string X509CertPath = FileStorageUtilities.FindKeyValuePair("CertificatePath", CertificateConfig).Value;
                    string password = FileStorageUtilities.FindKeyValuePair("Password", CertificateConfig).Value;
                    return new AzureKeyVaultX509CertificateService(new AzureCredentialsX509CertModel(Uri, TenantId, ClientId, X509CertPath, password));
                default:
                    string ClientSecret = FileStorageUtilities.FindKeyValuePair("ClientSecret", CredDetails).Value;
                    return new AzureKeyVaultCSService(new AzureCredentialsCSModel(Uri, TenantId, ClientId, ClientSecret));
            }
        }
    }

    public class AWSSecretServiceRetrieval : IRetrieveSecretService
    {
        public IBaseSecrets GetSecretService(List<IConfigurationSection> CredentialsConfig) => BuildAWSSecretService(CredentialsConfig);

        public static IBaseSecrets BuildAWSSecretService(List<IConfigurationSection> CredDetails)
        {
            string Profile = FileStorageUtilities.FindKeyValuePair("Profile", CredDetails).Value;
            string Region = FileStorageUtilities.FindKeyValuePair("Region", CredDetails).Value;
            throw new NotImplementedException();
            //return new AWSSecretService(new AWSSimpleCredentialsModel(Profile, Region));
        }
    }
}
