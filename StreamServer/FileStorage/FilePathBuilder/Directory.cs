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
        public FilePath CreateFilePath(string FileLocation);
    }

    public class CustomDirectory : IDirectory
    {
        public FilePath CreateFilePath(string FileLocation) => new FilePath(FileLocation);
    }

    public class LocalDirectory : IDirectory
    {
        public FilePath CreateFilePath(string FileLocation) => new CurrentDirectoryFilePath(FileLocation);
    }
}
