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
    public interface IFileStorageBuilder
    {
    }

    public class FileStorageManager
    {
        /* Physical File Storage Specific Location: Azure, AWS, Local  */
        public const string FSTypeKeyName = "Type";

        /*  Physical File Storage General Location
         *  - Don't see it ever being anything but either local or 
         *      external.
         */
        public enum FileStorageGeneralType
        {
            local,
            external
        }

        public IFileStorageModel model { get; }

        //  File Storage Configuration Data - Is it necesssary to store it?
        //  public List<IConfigurationSection> fileStorageConfig { get; }

        public FileStorageManager(List<IConfigurationSection> FileStorageConfig)
        {
            //fileStorageConfig = FileStorageConfig;
            string FileStorageType = FileStorageUtilities.FindKeyValuePair(FSTypeKeyName, FileStorageConfig).Value;

            //  If specfici type is not set to local e.g AWS, Azure etc... then assume it is an external service
            FileStorageGeneralType fileStoreType = (!Enum.TryParse(FileStorageType.ToLower(), out fileStoreType)) ? FileStorageGeneralType.external : FileStorageGeneralType.local;

            //  Separating logic from this managerial class that just dictates where 
            //  it should go (Don't see it ever growing beyong local or external)
            switch (fileStoreType)
            {
                case FileStorageGeneralType.local:
                    model = new LocalFileStorageModelBuilder(FileStorageConfig).fileStorageModel;
                    break;
                case FileStorageGeneralType.external:
                    model = new ExternalFileStorageModelBuilder(FileStorageConfig).fileStorageModel;
                    break;
            }
        }
    }
}
