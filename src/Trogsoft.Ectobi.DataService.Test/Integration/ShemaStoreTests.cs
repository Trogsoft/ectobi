using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Data;

namespace Trogsoft.Ectobi.DataService.Test.Integration
{
    public class ShemaStoreTests : TestBase
    {
        private EctoData store;

        [SetUp]
        public void Setup()
        {
            base.Setup();
            this.store = new EctoData(GetDatabase(), null, null, new EctoMapper());
        }

        [Test]
        public void SchemaStoreListSchemasEmpty()
        {
            var schemas = store.Schema.GetSchemas().Result;
            Assert.That(schemas.Count(), Is.EqualTo(0));
        }

        [Test]
        public void CreateEmptySchema()
        {
            const string schemaName = "Test Schema";
            var newSchema = store.Schema.CreateSchema(new Common.SchemaEditModel
            {
                Name = schemaName
            }).Result;
            Assert.That(newSchema, Is.Not.Null);
            Assert.That(newSchema.Name!, Is.EqualTo(schemaName));
        }

        [Test]
        public void CreateEmptySchemaAndDelete()
        {
            const string schemaName = "Test Schema";
            var newSchema = store.Schema.CreateSchema(new Common.SchemaEditModel
            {
                Name = schemaName
            }).Result;
            Assert.That(newSchema, Is.Not.Null);
            Assert.That(newSchema.Name!, Is.EqualTo(schemaName));

            store.Schema.DeleteSchema(newSchema.TextId).Wait();
            Assert.That(store.Schema.GetSchemas().Result.Count(), Is.EqualTo(0));
       }


    }
}
