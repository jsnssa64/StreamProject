using StreamServer.FileStorage.FSModel;
using StreamServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamServer.FileStorage.FilePathBuilder
{
    public interface IDirectory
    {
        public FilePath GetFilePath(string FileLocation);
    }

    public class CustomDirectory : IDirectory
    {
        public FilePath GetFilePath(string FileLocation) => new FilePath(FileLocation);
    }

    public class LocalDirectory : IDirectory
    {
        public FilePath GetFilePath(string FileLocation) => new CurrentDirectoryFilePath(FileLocation);
    }
}
