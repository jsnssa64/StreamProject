using Microsoft.Extensions.Configuration;
using StreamServer.FileStorage.FilePathBuilder;
using StreamServer.FileStorage.FSModel;
using StreamServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamServer.FileStorage.FSBuilder
{
    public interface IFileStorageModelBuilder
    {
        public IFileStorageModel fileStorageModel { get; }
    }
    public class ExternalFileStorageModelBuilder : IFileStorageModelBuilder
    {
        public const string CredentialsKeyName = "Credentials";
        public IFileStorageModel fileStorageModel { get; }

        public ExternalFileStorageModelBuilder(List<IConfigurationSection> Config)
        {
            //  With Secret
            //  Find, Create Credentials and Create Local Storage Object
            ExternalStorageType CredType = GetCredentialType(Config);

            List<IConfigurationSection> FileStorageCredentialConfig = FileStorageUtilities.FindKeyValuePair(CredentialsKeyName, Config).GetChildren().ToList();

            fileStorageModel = new ExternalFileStoreModel(CredType, FileStorageCredentialConfig);
        }



        //  Type = File Storage Location e.g aws S3
        public ExternalStorageType GetCredentialType(List<IConfigurationSection> Config) => Enum.Parse<ExternalStorageType>(FileStorageUtilities.FindKeyValuePair("Type", Config).Value.ToLower());
    }

    public class LocalFileStorageModelBuilder : IFileStorageModelBuilder
    {
        public IFileStorageModel fileStorageModel { get; }

        public const string FilePathKeyName = "FilePath";
        public const string TypeKeyName = "Type";
        public const string PlainFilePathStorageType = "Plain";

        public LocalFileStorageModelBuilder(List<IConfigurationSection> FileStorageConfig)
        {
            List<IConfigurationSection> filePathConfig = FileStorageUtilities.FindKeyValuePair(FilePathKeyName, FileStorageConfig).GetChildren().ToList();
            LocalFilePath PathModel = GetFilePath(filePathConfig);

            //  Build File Storage Model
            fileStorageModel = new LocalFileStoreModel("", PathModel.filePath);
        }

        //  File Path Builder (Secret/Plain)
        public LocalFilePath GetFilePath(List<IConfigurationSection> FilePathConfig) => FileStorageUtilities.FindKeyValuePair(TypeKeyName, FilePathConfig).Value == PlainFilePathStorageType ? new FilePathPlain(FilePathConfig) : new FilePathSecret(FilePathConfig);
    }
}
