using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.Ectobi.DataService.TemporaryStore;

namespace Trogsoft.Ectobi.DataService.Test.Unit
{
    internal class SystemTempStoreTests
    {

        TestModel testModel = new()
        {
            IsValid = true,
            ModelName = "Model",
            SchemaName = "Schema",
        };


        [Test]
        public void Create()
        {
            Assert.DoesNotThrow(() => new SystemTempStore());
        }

        [Test]
        public void CreateAndStoreJson()
        {
            var store = new SystemTempStore();
            var result = store.StoreObject(testModel);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void StoreAndDeleteJson()
        {
            var store = new SystemTempStore();
            var result = store.StoreObject(testModel);
            Assert.That(result, Is.Not.Null);
            Assert.DoesNotThrow(() => store.Remove(result));
        }

        [Test(Description = "Delete fails silently if the file doesn't exist.")]
        public void RemoveNonExistent()
        {
            var store = new SystemTempStore();
            Assert.DoesNotThrow(() => store.Remove(Guid.NewGuid().ToString()));
        }

        [Test]
        public void CreateStoreAndReadJson()
        {
            var store = new SystemTempStore();
            var result = store.StoreObject(testModel);
            Assert.That(result, Is.Not.Null);
            var retrieved = store.GetStoredObject<TestModel>(result);
            Assert.That(retrieved, Is.Not.Null);
            Assert.That(retrieved.SchemaName == testModel.SchemaName, Is.True);
            Assert.That(retrieved.ModelName == testModel.ModelName, Is.True);
        }

        [Test]
        public void ReadNonExistentStoredValue()
        {
            var store = new SystemTempStore();
            Assert.Throws<FileNotFoundException>(() =>
            {
                var retrieved = store.GetStoredObject<TestModel>(Guid.NewGuid().ToString());
            });
        }

        [Test]
        public void ReadNonExistentStoredFile()
        {
            var store = new SystemTempStore();
            Assert.Throws<FileNotFoundException>(() =>
            {
                var retrieved = store.GetBinaryFile(Guid.NewGuid().ToString());
            });
        }

        [Test]
        public void StoreBinaryFile()
        {
            var store = new SystemTempStore();
            var bin = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var result = store.StoreBinaryFile(bin);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void StoreAndRetrieveBinaryFile()
        {
            var store = new SystemTempStore();
            var bin = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var result = store.StoreBinaryFile(bin);
            Assert.That(result, Is.Not.Null);
            var retrieved = store.GetBinaryFile(result);
            Assert.That(retrieved, Is.Not.Null);
            Assert.That(retrieved.Length == 8, Is.True);
            Assert.That(retrieved[0] == 1, Is.True);
        }

        [Test]
        public void StoreAndDeleteBinaryFile()
        {
            var store = new SystemTempStore();
            var bin = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var result = store.StoreBinaryFile(bin);
            Assert.That(result, Is.Not.Null);
            Assert.DoesNotThrow(() => store.Remove(result));
        }


    }
}
