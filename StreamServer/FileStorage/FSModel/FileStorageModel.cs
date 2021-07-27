using Microsoft.Extensions.Configuration;
using StreamServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamServer.FileStorage.FSModel
{
    public interface IFileStorageModel {
        public void GetFile(List<IConfigurationSection> CredentialConfig);
    }

    public class LocalFileStoreModel : IFileStorageModel
    {
        public string fileName { get; }
        public FilePath filePath { get; }
        public LocalFileStoreModel(string FileName, FilePath FilePath)
        {
            fileName = FileName;
            filePath = FilePath;
        }

        //  TODO
        //  Doesn't use Credentials
        public void GetFile(List<IConfigurationSection> CredentialConfig)
        {
            throw new NotImplementedException();
        }
    }

    public class ExternalFileStoreModel : IFileStorageModel
    {
        public ExternalStorageType type { get; }
        public List<IConfigurationSection> credentialConfig { get; }

        //  TODO
        //  Need To Pass Credentials Configuration
        //  CredentialConfig= FileStorageUtilities.FindKeyValuePair("Credentials", Config).GetChildren().ToList();
        public ExternalFileStoreModel(ExternalStorageType Type, List<IConfigurationSection> CredentialConfig)
        {
            type = Type;
            credentialConfig = CredentialConfig;
        }

        public void GetFile(List<IConfigurationSection> CredentialConfig)
        {

            switch (type)
            {
                case ExternalStorageType.azure:
                    string AzureSecretType = FileStorageUtilities.FindKeyValuePair("Type", CredentialConfig).Value.ToLower();
                    FileStorageUtilities.AzureFileServiceManager(AzureSecretType, CredentialConfig);
                    break;
                case ExternalStorageType.aws:
                    FileStorageUtilities.AWSFileServiceManager(CredentialConfig);
                    break;
                default:
                    throw new Exception("Credentials Not Found");
            }

            throw new NotImplementedException();
        }
    }

}
