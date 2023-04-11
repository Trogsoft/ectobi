using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Internal;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;
using System.Collections;

namespace Trogsoft.Ectobi.DataService
{
    public class LocalConfigFileProvider : IFileProvider
    {

        string path;

        public string LocalConfigPath { get => path; }

        public LocalConfigFileProvider()
        {
            var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path = Path.Combine(root, "Ectobi", "Config");
            Directory.CreateDirectory(path);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var dirContents = new PhysicalDirectoryContents(Path.Combine(path, subpath));
            return dirContents;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var fileInfo = new PhysicalFileInfo(new FileInfo(Path.Combine(path, subpath)));
            return fileInfo;
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }

}
