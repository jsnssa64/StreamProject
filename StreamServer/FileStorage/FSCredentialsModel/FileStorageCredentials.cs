using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamServer.FileStorage.FileStorageCredentialsModel
{
    public interface FileStorageCredential
    {
    }

    public class AzureFileStorageCredentialsModel : FileStorageCredential
    {

    }

    public class AWSFileStorageCredentialsModel : FileStorageCredential
    {

    }

}
