using Microsoft.Extensions.Configuration;
using StreamServer.FileStorage.FilePathBuilder;
using StreamServer.FileStorage.FSBuilder;
using StreamServer.FileStorage.FSModel;
using StreamServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamServer.FileStorage.FSManager
{
    /*
     * FileStorageManager
     * Begins the creation  
     */

    public class FileStorageManager
    {
        /* Physical File Storage Specific Location: Azure, AWS, Local  */
        private const string FSTypeKeyName = "Type";

        public IFileStorageModel fileStorageModel { get { return modelBuilder.FileStorageModel; } }
        private IFileStorageModelBuilder modelBuilder { get; }

        public FileStorageManager(List<IConfigurationSection> FileStorageConfig)
        {
            string FileStorageType = FileStorageUtilities.FindKeyValuePair(FSTypeKeyName, FileStorageConfig).Value;

            //  Separating logic from this managerial class which sole purpose is to dictate where 
            //  it should go (Don't see it ever growing beyong local or external)
            modelBuilder = (FileStorageType.ToLower() == "local") ? new LocalFileStorageModelBuilder(FileStorageConfig) : new ExternalFileStorageModelBuilder(FileStorageConfig);
        }
    }
}
