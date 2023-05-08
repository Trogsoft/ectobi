using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.DataService.Validation;

namespace Trogsoft.Ectobi.DataService.Test.Unit
{
    internal class ModelValidatorTests
    {

        TestModel model = new()
        {
            SchemaName = "test-schema",
            ModelName = "Name",
            Value = null!,
            IsValid = true
        };

        [Test(Description = "Test whether passing the model and doing no other validation succeeds.")]
        public void BasicCreation()
        {
            var validator = EctoModelValidator.CreateValidator<TestModel>()
                .WithModel(model);

            Assert.That(validator.Validate(), Is.EqualTo(true));
        }

        [Test]
        public void BasicCreationNull()
        {
            var validator = EctoModelValidator.CreateValidator<TestModel>()
                .WithModel(null!);

            Assert.That(validator.Validate(), Is.EqualTo(false));
        }

        [Test]
        public void BasicCreationNoModel()
        {
            var validator = (ICanValidate<TestModel>)EctoModelValidator.CreateValidator<TestModel>();
            Assert.That(validator.Validate(), Is.EqualTo(false));
        }

        [Test]
        public void ValidateProperty()
        {
            var validator = EctoModelValidator.CreateValidator<TestModel>()
                .WithModel(model)
                .Property(x => x.SchemaName).NotNullOrWhiteSpace();

            Assert.That(validator.Validate(), Is.EqualTo(true));
        }

        [Test]
        public void ValidateEmptyProperty()
        {
            var validator = EctoModelValidator.CreateValidator<TestModel>()
                .WithModel(model)
                .Property(x => x.Value).NotNullOrWhiteSpace();

            Assert.That(validator.Validate(), Is.EqualTo(false));
        }

        [Test]
        public void ValidatePropertyLength()
        {
            var validator = EctoModelValidator.CreateValidator<TestModel>()
                .WithModel(model)
                .Property(x => x.ModelName).MinimumLength(5);

            Assert.That(validator.Validate(), Is.EqualTo(false));
        }

        [Test]
        public void ValidatePropertyLength2()
        {
            var validator = EctoModelValidator.CreateValidator<TestModel>()
                .WithModel(model)
                .Property(x => x.ModelName).MinimumLength(4);

            Assert.That(validator.Validate(), Is.EqualTo(true));
        }

        [Test]
        public void ValidatePropertyLengthMax()
        {
            var validator = EctoModelValidator.CreateValidator<TestModel>()
                .WithModel(model)
                .Property(x => x.ModelName).MaximumLength(3);

            Assert.That(validator.Validate(), Is.EqualTo(false));
        }


        [Test]
        public void ValidatePropertyLengthMax2()
        {
            var validator = EctoModelValidator.CreateValidator<TestModel>()
                .WithModel(model)
                .Property(x => x.ModelName).MaximumLength(5);

            Assert.That(validator.Validate(), Is.EqualTo(true));
        }


        [Test]
        public void TestBoolValueTypeMismatch()
        {
            var validator = EctoModelValidator.CreateValidator<TestModel>()
                .WithModel(model)
                .Property(x => x.IsValid).MinimumLength(4);

            Assert.That(validator.Validate(), Is.EqualTo(false));

            var result = validator.GetResult();
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCodes.ERR_TYPE_MISMATCH));

            var typedResult = validator.GetResult<string>();
            Assert.That(typedResult.ErrorCode, Is.EqualTo(ErrorCodes.ERR_TYPE_MISMATCH));
        }

    }
}
