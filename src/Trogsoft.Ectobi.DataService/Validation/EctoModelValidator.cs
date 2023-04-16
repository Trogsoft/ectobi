using ClosedXML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Validation
{

    public static class EctoModelValidator
    {

        public static ICanAddModel<T> CreateValidator<T>() => new EctoModelValidator<T>();
        public static ICanAddModel<T> CreateValidator<T>(EctoDb db) => new EctoModelValidator<T>(db);

    }

    public interface ICanAddModel<T>
    {
        IValidatorFirstStage<T> WithModel(T model);
    }

    public interface IValidatorFirstStage<T> : ICanValidate<T>
    {
        IValidatorPropertyOptions<T> Property<TProp>(Func<T, TProp> selector);
        IValidatorEntityOptions<T> Entity<TEntity>(Func<T, string> selector) where TEntity : NamedEntity;
    }

    public interface IValidatorEntityOptions<T> : IValidatorFirstStage<T>, ICanValidate<T>
    {
        IValidatorEntityOptions<T> MustExist();
        IValidatorEntityOptions<T> Get<TEntity>(out TEntity value);
    }

    public interface IValidatorPropertyOptions<T> : IValidatorFirstStage<T>, ICanValidate<T>
    {
        IValidatorPropertyOptions<T> NotNullOrWhiteSpace();
        IValidatorPropertyOptions<T> MinimumLength(int length);
        IValidatorPropertyOptions<T> MaximumLength(int length);
    }

    public interface ICanValidate<T>
    {
        bool Validate();
        Success<TResult> GetResult<TResult>();
    }

    public class ValidationError
    {
        public int ErrorCode { get; set; } = 0;
        public string? Message { get; set; }
    }

    public class EctoModelValidator<T> :
        ICanAddModel<T>,
        IValidatorFirstStage<T>,
        IValidatorEntityOptions<T>,
        IValidatorPropertyOptions<T>,
        ICanValidate<T>
    {

        private readonly EctoDb? db;

        private T? Model;

        List<ValidationError> errors = new List<ValidationError>();
        private bool Failed = false;

        private string? propertyName;
        private object? property;
        private object? entity;

        private EctoModelValidator<T> RecordValidationError(int errorCode, string? message, bool stop = false)
        {
            errors.Add(new ValidationError { ErrorCode = errorCode, Message = message });
            if (stop) this.Failed = true;
            return this;
        }

        private bool hasFailed() => Failed;

        public EctoModelValidator() {
        }

        public EctoModelValidator(EctoDb db)
        {
            this.db = db;
        }

        IValidatorFirstStage<T> ICanAddModel<T>.WithModel(T model)
        {
            this.Model = model;
            if (model == null) RecordValidationError(ErrorCodes.ERR_ARGUMENT_NULL, "Model cannot be null.", true);
            return this;
        }

        IValidatorPropertyOptions<T> IValidatorFirstStage<T>.Property<TProp>(Func<T, TProp> selector)
        {
            if (hasFailed()) return this;
            this.propertyName = selector.GetInvocationList().Last().ToString();
            this.property = selector.Invoke(Model);
            return this;
        }

        IValidatorEntityOptions<T> IValidatorFirstStage<T>.Entity<TEntity>(Func<T, string> idSelector) 
        { 
            if (hasFailed()) return this;
            var id = idSelector.Invoke(Model);
            this.entity = db.Set<TEntity>().SingleOrDefault(x=>x.TextId == id);
            return this;
        }

        IValidatorPropertyOptions<T> IValidatorPropertyOptions<T>.NotNullOrWhiteSpace()
        {
            if (hasFailed()) return this;
            if (property == null || (property is string str && string.IsNullOrWhiteSpace(str)))
                RecordValidationError(ErrorCodes.ERR_ARGUMENT_NULL, $"{propertyName ?? "Property"} cannot be null.");
            return this;
        }

        IValidatorPropertyOptions<T> IValidatorPropertyOptions<T>.MinimumLength(int length)
        {
            if (hasFailed()) return this;
            if (property is string str)
            {
                if (str.Length < length)
                    return RecordValidationError(ErrorCodes.ERR_VALUE_TOO_SHORT, $"{propertyName ?? "Property"} is must be more than {length} characters.");
            }
            else
            {
                return RecordValidationError(ErrorCodes.ERR_TYPE_MISMATCH, $"Type mismatch: {propertyName ?? "Property"} is not a string.");
            }
            return this;
        }

        IValidatorPropertyOptions<T> IValidatorPropertyOptions<T>.MaximumLength(int length)
        {
            if (hasFailed()) return this;
            if (property is string str)
            {
                if (str.Length > length)
                    return RecordValidationError(ErrorCodes.ERR_VALUE_TOO_LONG, $"{propertyName ?? "Property"} must be less than {length} characters.");
            }
            else
            {
                return RecordValidationError(ErrorCodes.ERR_TYPE_MISMATCH, $"Type mismatch: {propertyName ?? "Property"} is not a string.");
            }
            return this;
        }

        IValidatorEntityOptions<T> IValidatorEntityOptions<T>.MustExist()
        {
            if (hasFailed()) return this;
            if (entity == null) return RecordValidationError(ErrorCodes.ERR_NOT_FOUND, "Entity not found.");
            return this;
        }

        IValidatorEntityOptions<T> IValidatorEntityOptions<T>.Get<TEntity>(out TEntity value)
        {
            value = default!;
            if (hasFailed()) return this;
            value = (TEntity)entity!;
            return this;
        }

        bool ICanValidate<T>.Validate() => !Failed;

        Success<TResult> ICanValidate<T>.GetResult<TResult>()
        {
            var firstError = errors.FirstOrDefault();
            if (Failed) 
                return new Success<TResult>(firstError?.ErrorCode ?? ErrorCodes.ERR_UNSPECIFIED_ERROR, firstError?.Message ?? "Unknown error.");
            else
                return new Success<TResult>(true, firstError?.Message ?? null!);
        }

    }

}
