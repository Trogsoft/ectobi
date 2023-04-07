using Newtonsoft.Json;
using System.Collections;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.TemporaryStore
{
    public class SystemTempStore : ITemporaryStore
    {

        string tempPath;

        public SystemTempStore() {
            tempPath = Path.Combine(Path.GetTempPath(), "Ectobi");
            Directory.CreateDirectory(tempPath);
        }

        public string StoreBinaryFile(byte[] byteArray)
        {
            Guid guid = Guid.NewGuid();
            var path = Path.Combine(tempPath, guid.ToString() + ".tmp");
            File.WriteAllBytes(path, byteArray);
            return guid.ToString();
        }

        public string StoreObject<T>(T objectToStore)
        {
            Guid guid = Guid.NewGuid();
            var path = Path.Combine(tempPath, guid.ToString() + ".tmp");
            File.WriteAllText(path, JsonConvert.SerializeObject(objectToStore));
            return guid.ToString();
        }

        public byte[] GetBinaryFile(string storedFileId)
        {
            var path = Path.Combine(tempPath, storedFileId + ".tmp");
            if (!File.Exists(path)) throw new FileNotFoundException();
            return File.ReadAllBytes(path);
        }

        public T GetStoredObject<T>(string storedFileId)
        {
            var path = Path.Combine(tempPath, storedFileId + ".tmp");
            if (!File.Exists(path)) throw new FileNotFoundException();
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Remove(string temporaryFile)
        {
            var path = Path.Combine(tempPath, temporaryFile + ".tmp");
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
