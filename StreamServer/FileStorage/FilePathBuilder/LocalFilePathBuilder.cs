using Microsoft.Extensions.Configuration;
using SecretServices;
using SecretServices.SecretServiceBuilder;
using SecretServices.Services;
using StreamServer.FileStorage.FSModel;
using StreamServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamServer.FileStorage.FilePathBuilder
{
    public abstract class LocalFilePathBuilder
    {
        public const string CurrentDirectoryKeyName = "CurrentDirectory";
        public const string FilePathKeyName = "FilePath";
        public IDirectory directory { get; }
        public string Path { get; set; }

        public LocalFilePathBuilder(List<IConfigurationSection> filePathConfig)
        {
            bool UseCurrentDirectory = bool.Parse(FileStorageUtilities.FindKeyValuePair(CurrentDirectoryKeyName, filePathConfig).Value);
            directory = GetDirectoryType(UseCurrentDirectory);
        }

        //  There will only ever be two options
        //  Therefore No need for class separation
        public IDirectory GetDirectoryType(bool EnableCurrentDirectory) => EnableCurrentDirectory ? new LocalDirectory() : new CustomDirectory();

        public FilePath Build(string Path)
        {
            return directory.CreateFilePath(Path);
        }
    }

    /*
     * Development Only
     */
    public class FilePathPlainBuilder : LocalFilePathBuilder
    {
        public const string PathKeyName = "Path";
        public FilePathPlainBuilder(List<IConfigurationSection> filePathConfig) : base(filePathConfig)
        {
            Path = FileStorageUtilities.FindKeyValuePair(PathKeyName, filePathConfig).Value;

            Console.WriteLine("Plain -> FileLocation:");
            Console.WriteLine(Path);
        }
    }

    /*
     *  Get File Path using External Secret Service e.g AWS, Azure etc...
     */
    public class FilePathSecretBuilder : LocalFilePathBuilder
    {
        //  Add Here Any External Secret Handlers...
        public enum SecretType
        {
            aws,
            azure
        }

        public const string SecretsCredentialsTypeKeyName = "Type";
        public const string FilePathCredentialsKeyName = "Credentials";
        public const string SecretServiceKeyName = "StreamServerFilePath";
        public IBaseSecrets secretsService { get; }
        public SecretType SecretServiceType { get; }

        public FilePathSecretBuilder(List<IConfigurationSection> FilePathConfig) : base(FilePathConfig)
        {
            List<IConfigurationSection> SecretsCredentialsConfig = FileStorageUtilities.FindKeyValuePair(FilePathCredentialsKeyName, FilePathConfig).GetChildren().ToList();

            string SecretType = FileStorageUtilities.FindKeyValuePair(SecretsCredentialsTypeKeyName, SecretsCredentialsConfig).Value; 
            SecretServiceType = ParseSecretType(SecretType);

            //  Run and Retrieve FilePath From Secret Service
            secretsService = GetSecretService(SecretServiceType, SecretsCredentialsConfig);
            Path = GetFilePath(secretsService);

            Console.WriteLine("Secret -> FileLocation:");
            Console.WriteLine(Path);
        }


        /*  UPDATE HERE: If more services are used Update here */
        public IBaseSecrets GetSecretService(SecretType SecretType, List<IConfigurationSection> CredentialsConfig)
        {
            switch (SecretType)
            {
                case SecretType.azure:
                    return BuildAzureSecretService(CredentialsConfig);
                case SecretType.aws:
                    return BuildAWSSecretService(CredentialsConfig);
                default:
                    throw new Exception("Credentials Not Found");
            }
        }

        public IBaseSecrets BuildAzureSecretService(List<IConfigurationSection> CredentialsConfig)
        {
            string AzureCredentialsType =   FileStorageUtilities.FindKeyValuePair("Type", CredentialsConfig).Value;
            string Uri =                    FileStorageUtilities.FindKeyValuePair("Uri", CredentialsConfig).Value;
            string TenantId =               FileStorageUtilities.FindKeyValuePair("TenantId", CredentialsConfig).Value;
            string ClientId =               FileStorageUtilities.FindKeyValuePair("ClientId", CredentialsConfig).Value;

            switch (AzureCredentialsType.ToLower())
            {
                case "certificate":
                    string CertPath = FileStorageUtilities.FindKeyValuePair("CertificatePath", CredentialsConfig).Value;
                    return new AzureBasicCertificateBuilder(Uri, TenantId, ClientId, CertPath).Service;
                case "X509":
                    List<IConfigurationSection> CertDetailsConfig = FileStorageUtilities.FindKeyValuePair("Certificate", CredentialsConfig).GetChildren().ToList();
                    string X509CertPath = FileStorageUtilities.FindKeyValuePair("CertificatePath", CertDetailsConfig).Value;
                    string password = FileStorageUtilities.FindKeyValuePair("Password", CertDetailsConfig).Value;
                    return new AzureX509CertificateBuilder(Uri, TenantId, ClientId, X509CertPath, password).Service;
                default:
                    string ClientSecret = FileStorageUtilities.FindKeyValuePair("ClientSecret", CredentialsConfig).Value;
                    return new AzureClientSecretBuilder(Uri, TenantId, ClientId, ClientSecret).Service;
            }
        }

        public IBaseSecrets BuildAWSSecretService(List<IConfigurationSection> CredentialsConfig)
        {
            string Profile =    FileStorageUtilities.FindKeyValuePair("Profile", CredentialsConfig).Value;
            string Region =     FileStorageUtilities.FindKeyValuePair("Region", CredentialsConfig).Value;
            return new AWSSecretServiceBuilder(Profile, Region).BuildAWSSecretService();
        }

        private string GetFilePath(IBaseSecrets SecretsService) => SecretsService.GetValue(SecretServiceKeyName);

        //  Get Secret Handlers
        public SecretType ParseSecretType(string Type) => Enum.Parse<SecretType>(Type.ToLower());

    }
}
