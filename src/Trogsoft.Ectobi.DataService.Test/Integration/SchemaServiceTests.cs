using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;
using Moq.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Data;
using Trogsoft.Ectobi.DataService.Interfaces;
using Trogsoft.Ectobi.DataService.Services;

namespace Trogsoft.Ectobi.DataService.Test.Integration
{
    [TestFixture]
    public class SchemaServiceTests : TestBase
    {
        private SchemaService schemaSvc;

        [SetUp]
        public override void Setup()
        {

            base.Setup();

            var logger = new Mock<ILogger<SchemaService>>();
            var fieldLogger = new Mock<ILogger<FieldService>>();
            var moduleLogger = new Mock<ILogger<ModuleManager>>();
            var backgroundTaskCoordinator = new Mock<IBackgroundTaskCoordinator>();
            var webHookService = new Mock<IWebHookService>();
            var lookupService = new Mock<ILookupService>();
            var ifts = new Mock<IFileTranslatorService>();
            var opts = new Mock<IOptions<ModuleOptions>>();
            var mapper = new Mock<EctoMapper>().Object;
            var fieldService = new Mock<IFieldBackgroundService>().Object;
            var db = GetDatabase();
            var data = new EctoData(db, webHookService.Object, null, mapper);

            schemaSvc = new SchemaService(logger.Object, db, mapper, fieldService, lookupService.Object, webHookService.Object, ifts.Object, data);

        }

        [Test]
        public void GetSchemas()
        {
            var result = schemaSvc.GetSchemas().Result;
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Result);
            Assert.That(result.Result.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetMissingSchema()
        {
            var result = schemaSvc.GetSchema("missing").Result;
            Assert.False(result.Succeeded);
            Assert.True(result.ErrorCode == ErrorCodes.ERR_NOT_FOUND);
            Assert.Null(result.Result);
        }

        [Test]
        public void CreateEmptySchema()
        {
            var result = schemaSvc.CreateSchema(new SchemaEditModel
            {
                Name = "Empty Schema"
            }).Result;
            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.Result, Is.Not.Null);
                Assert.That(result.Result!.Name, Is.EqualTo("Empty Schema"));
                Assert.That(result.Result.TextId, Is.EqualTo("empty-schema"));
                Assert.That(result.Result.Id, Is.GreaterThan(0));
            });
        }

        [Test]
        public void CreateEmptySchemaAndCheckExistence()
        {
            CreateEmptySchema();
            var queryResult = schemaSvc.GetSchema("empty-schema").Result;
            Assert.Multiple(() =>
            {
                Assert.That(queryResult.Succeeded, Is.True);
                Assert.That(queryResult.Result, Is.Not.Null);
                Assert.That(queryResult.Result!.Id, Is.GreaterThan(0));
                Assert.That(queryResult.Result.Name, Is.EqualTo("Empty Schema"));
            });
        }

        [Test]
        public void CreateSchemaAndDelete()
        {
            CreateEmptySchema();
            var result = schemaSvc.DeleteSchema("empty-schema").Result;
            Assert.True(result.Succeeded);

            var countResult = schemaSvc.GetSchemas().Result;
            Assert.Multiple(() =>
            {
                Assert.That(countResult.Succeeded, Is.True);
                Assert.That(countResult.Result, Is.Not.Null);
                Assert.That(countResult.Result!.Count, Is.EqualTo(0));
            });
        }

        [Test]
        public void CreateSchemaWithFields()
        {
            var result = schemaSvc.CreateSchema(new SchemaEditModel
            {
                Name = "Test Schema",
                Fields = new List<SchemaFieldEditModel>
                 {
                     new SchemaFieldEditModel { Name = "Name", },
                     new SchemaFieldEditModel { Name = "Address" },
                     new SchemaFieldEditModel { Name = "Email Address" }
                 }
            }).Result;
            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.Result, Is.Not.Null);
            });
            Assert.That(result.Result!.Name, Is.EqualTo("Test Schema"));
            Assert.That(result.Result.Fields.Count, Is.EqualTo(3));
        }
    }
}
