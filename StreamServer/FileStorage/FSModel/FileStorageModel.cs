using Microsoft.Extensions.Configuration;
using StreamServer.FileStorage.FileStorageCredentialsModel;
using StreamServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamServer.FileStorage.FSModel
{
    public interface IFileStorageModel
    {
        public string storageType { get; }
    }

    public class LocalFileStorageModel : IFileStorageModel
    {
        public FilePath filePath { get; }
        public string storageType { get; }

        public LocalFileStorageModel(string StorageType, FilePath FilePath)
        {
            storageType = StorageType;
            filePath = FilePath;
        }
    }

    public class ExternalFileStorageModel : IFileStorageModel
    {
        public string storageType { get; }

        public FileStorageCredential storageCredentials { get; }

        public ExternalFileStorageModel(string StorageType, FileStorageCredential StorageCredentials)
        {
            storageType = StorageType;
            storageCredentials = StorageCredentials;
        }
    }

}
