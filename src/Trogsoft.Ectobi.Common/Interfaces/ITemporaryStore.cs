using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface ITemporaryStore
    {
        byte[] GetBinaryFile(string storedFileId);
        T GetStoredObject<T>(string storedFileId);
        void Remove(string temporaryFile);
        string StoreBinaryFile(byte[] byteArray);
        string StoreObject<T>(T objectToStore);
    }
}
