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
    //  Even though similar keep both separate
    //  as you might add additonal ways of accessing secrets
    //  but not adding any other ways of accessing Files
    public enum FileStorageCredentialType
    {
        azure,
        aws,
        local
    }

    public enum FilePathSecretsCredentialType
    {
        azure,
        aws,
        plain
    }

    public interface IFileStorageModelBuilder
    {
        public const string FileStorageTypeKeyName = "Type";
        public IFileStorageModel FileStorageModel { get; }
    }
    public class ExternalFileStorageModelBuilder : IFileStorageModelBuilder
    {
        public const string CredentialsKeyName = "Credentials";
        public const string CredentialsTypeKeyName = "Type";
        public IFileStorageModel FileStorageModel { get; }

        public ExternalFileStorageModelBuilder(List<IConfigurationSection> FileStorageConfig)
        {
            //  With Secret
            //  Find, Create Credentials and Create Local Storage Object
            FileStorageCredentialType CredType = GetCredentialType(FileStorageConfig);

            List<IConfigurationSection> FileStorageCredentialConfig = FileStorageUtilities.FindKeyValuePair(CredentialsKeyName, FileStorageConfig).GetChildren().ToList();

            FileStorageModel = new ExternalFileStorageModel(CredType, FileStorageCredentialConfig);
        }



        //  Type = File Storage Location e.g aws S3
        public FileStorageCredentialType GetCredentialType(List<IConfigurationSection> CredentialsConfig) => Enum.Parse<FileStorageCredentialType>(FileStorageUtilities.FindKeyValuePair(CredentialsTypeKeyName, CredentialsConfig).Value.ToLower());
    }

    public class LocalFileStorageModelBuilder : IFileStorageModelBuilder
    {
        public IFileStorageModel FileStorageModel { get; }

        public const string TypeKeyName = "Type";
        public const string FilePathKeyName = "FilePath";
        public const string FilePathTypeKeyName = "Type";
        public const string CredentialsTypeKeyName = "Type";
        public const string PlainFilePathStorageType = "Plain";

        public LocalFileStorageModelBuilder(List<IConfigurationSection> FileStorageConfig)
        {
            string TypeValue = FileStorageUtilities.FindKeyValuePair(TypeKeyName, FileStorageConfig).Value;

            //  Generate File Path
            List<IConfigurationSection> filePathConfig = FileStorageUtilities.FindKeyValuePair(FilePathKeyName, FileStorageConfig).GetChildren().ToList();
            LocalFilePathBuilder PathModel = GetFilePathBuilder(filePathConfig);
            
            //  Build File Storage Model
            FileStorageModel = new LocalFileStorageModel(TypeValue, PathModel.Build(PathModel.Path));
        }

        //  Get File Path Builder (Secret/Plain) and build
        public LocalFilePathBuilder GetFilePathBuilder(List<IConfigurationSection> FilePathConfig) => FileStorageUtilities.FindKeyValuePair(FilePathTypeKeyName, FilePathConfig).Value == PlainFilePathStorageType ? new FilePathPlainBuilder(FilePathConfig) : new FilePathSecretBuilder(FilePathConfig);

        //  Type = File Storage Location e.g aws S3
        public FileStorageCredentialType GetCredentialType(List<IConfigurationSection> CredentialsConfig) => Enum.Parse<FileStorageCredentialType>(FileStorageUtilities.FindKeyValuePair(CredentialsTypeKeyName, CredentialsConfig).Value.ToLower());

    }
}
