using Microsoft.Extensions.Configuration;
using SecretServices;
using StreamServer.FileStorage.FSModel;
using StreamServer.FileStorage.SecretServiceRetrieval;
using StreamServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamServer.FileStorage.FilePathBuilder
{
    public abstract class LocalFilePath
    {
        public const string CurrentDirectoryKeyName = "CurrentDirectory";
        public const string FilePathKeyName = "FilePath";
        public FilePath filePath { get; set; }
        public IDirectory directory { get; }

        public LocalFilePath(List<IConfigurationSection> filePathConfig)
        {
            bool UseCurrentDirectory = bool.Parse(FileStorageUtilities.FindKeyValuePair(CurrentDirectoryKeyName, filePathConfig).Value);
            directory = GetDirectoryType(UseCurrentDirectory);
        }

        //  There will only ever be two options
        //  Therefore No need for class separation
        public IDirectory GetDirectoryType(bool EnableCurrentDirectory) => EnableCurrentDirectory ? new LocalDirectory() : new CustomDirectory();
    }

    /*
     * Development Only
     */
    public class FilePathPlain : LocalFilePath
    {
        public const string PathKeyName = "Path";
        public FilePathPlain(List<IConfigurationSection> filePathConfig) : base(filePathConfig)
        {
            string Path = FileStorageUtilities.FindKeyValuePair(PathKeyName, filePathConfig).Value;

            Console.WriteLine("Plain -> FileLocation:");
            Console.WriteLine(Path);

            filePath = directory.GetFilePath(Path);
        }
    }

    /*
     *  Get File Path using External Secret Service e.g AWS, Azure etc...
     */
    public class FilePathSecret : LocalFilePath
    {
        //  Add Here Any External Secret Handlers...
        public enum FilePathSecretType
        {
            aws,
            azure
        }

        public const string FilePathCredentialsKeyName = "Credentials";
        public const string SecretServiceKeyName = "StreamServerFilePath";

        public IBaseSecrets secretsService { get; }
        public FilePathSecret(List<IConfigurationSection> FilePathConfig) : base(FilePathConfig)
        {
            List<IConfigurationSection> SecretsCredentialsConfig = FileStorageUtilities.FindKeyValuePair(FilePathCredentialsKeyName, FilePathConfig).GetChildren().ToList();

            FilePathSecretType CredentialType = GetCredentialType(SecretsCredentialsConfig);

            //  Run and Retrieve FilePath From Secret Service
            secretsService = GetSecretService(CredentialType, SecretsCredentialsConfig);
            string Path = GetFilePath(secretsService);

            Console.WriteLine("Secret -> FileLocation:");
            Console.WriteLine(Path);

            filePath = directory.GetFilePath(Path);
        }


        /*  UPDATE HERE: If more services are used Update here */
        public IBaseSecrets GetSecretService(FilePathSecretType CredentialType, List<IConfigurationSection> CredentialsConfig)
        {
            switch (CredentialType)
            {
                case FilePathSecretType.azure:
                    return new AzureSecretServiceRetrieval().GetSecretService(CredentialsConfig);
                case FilePathSecretType.aws:
                    return new AWSSecretServiceRetrieval().GetSecretService(CredentialsConfig);
                default:
                    throw new Exception("Credentials Not Found");
            }
        }

        private string GetFilePath(IBaseSecrets SecretsService) => SecretsService.GetValue(SecretServiceKeyName);

        //  Get Secret Handlers
        public FilePathSecretType GetCredentialType(List<IConfigurationSection> Config) => Enum.Parse<FilePathSecretType>(FileStorageUtilities.FindKeyValuePair("Type", Config).Value.ToLower());

    }
}
