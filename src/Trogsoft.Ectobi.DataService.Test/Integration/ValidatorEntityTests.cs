using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Test.Unit;
using Trogsoft.Ectobi.DataService.Validation;

namespace Trogsoft.Ectobi.DataService.Test.Integration
{
    internal class ValidatorEntityTests : TestBase
    {

        TestModel model = new()
        {
            SchemaName = "test-schema",
            ModelName = "Name",
            Value = null!,
            IsValid = true
        };

        [Test]
        public void EntityValidatorNoDatbabasePassed()
        {
            var validator = EctoModelValidator.CreateValidator<TestModel>()
                .WithModel(model)
                .Entity<Schema>(x => x.SchemaName).MustExist();

            Assert.That(validator.Validate(), Is.EqualTo(false));
            var result = validator.GetResult();

            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCodes.ERR_ARGUMENT_NULL));
        }

        [Test]
        public void EntityValidatorMustExistFail()
        {
            var validator = EctoModelValidator.CreateValidator<TestModel>(GetDatabase())
                .WithModel(model)
                .Entity<Schema>(x => x.SchemaName).MustExist();

            Assert.That(validator.Validate(), Is.EqualTo(false));
            var result = validator.GetResult();

            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCodes.ERR_NOT_FOUND));
        }

        [Test]
        public void EntityValidatorMustNotExist()
        {
            var validator = EctoModelValidator.CreateValidator<TestModel>(GetDatabase())
                .WithModel(model)
                .Entity<Schema>(x => x.SchemaName).MustNotExist();

            Assert.That(validator.Validate(), Is.EqualTo(true));
        }
    }
}
