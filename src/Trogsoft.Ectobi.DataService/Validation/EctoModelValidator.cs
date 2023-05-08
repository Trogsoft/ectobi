using ClosedXML;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ObjectiveC;
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

    public class EctoModelValidator<TModel> :
        ICanAddModel<TModel>,
        IValidatorFirstStage<TModel>,
        IValidatorValueOptions<TModel>,
        IValidatorEntityOptions<TModel>,
        IValidatorPropertyOptions<TModel>,
        ICanValidate<TModel>
    {

        private readonly EctoDb? db;

        private TModel? Model;

        List<ValidationError> errors = new List<ValidationError>();
        private bool Failed = false;

        private string? propertyName;
        private object? property;
        private object? entity;

        private object? value;
        private object? valueName;

        private EctoModelValidator<TModel> RecordValidationError(int errorCode, string? message, bool stop = false)
        {
            errors.Add(new ValidationError { ErrorCode = errorCode, Message = message });
            if (stop) this.Failed = true;
            return this;
        }

        private bool hasFailed() => Model == null || Failed;

        public EctoModelValidator()
        {
        }

        public EctoModelValidator(EctoDb db)
        {
            this.db = db;
        }

        IValidatorFirstStage<TModel> ICanAddModel<TModel>.WithModel(TModel model)
        {
            this.Model = model;
            if (model == null) RecordValidationError(ErrorCodes.ERR_ARGUMENT_NULL, "Model cannot be null.", true);
            return this;
        }

        IValidatorPropertyOptions<TModel> IValidatorFirstStage<TModel>.Property<TProp>(Expression<Func<TModel, TProp>> selector)
        {
            if (hasFailed()) return this;
            this.propertyName = selector.Name;
            this.property = selector.Compile().Invoke(Model);
            return this;
        }

        IValidatorEntityOptions<TModel> IValidatorFirstStage<TModel>.Entity<TEntity>(Func<TModel, string> idSelector)
        {
            if (hasFailed()) return this;
            if (db == null) return RecordValidationError(ErrorCodes.ERR_ARGUMENT_NULL, "Database was not passed.", true);
            var id = idSelector.Invoke(Model);
            this.entity = db.Set<TEntity>().SingleOrDefault(x => x.TextId == id);
            return this;
        }

        IValidatorEntityOptions<TModel> IValidatorFirstStage<TModel>.Entity<TEntity>(Func<TEntity, bool> selector)
        {
            if (hasFailed()) return this;
            if (db == null) return RecordValidationError(ErrorCodes.ERR_ARGUMENT_NULL, "Database was not passed.", true);
            this.entity = db.Set<TEntity>().SingleOrDefault(selector);
            return this;
        }


        IValidatorPropertyOptions<TModel> IValidatorPropertyOptions<TModel>.NotNullOrWhiteSpace()
        {
            if (hasFailed()) return this;
            if (property == null || (property is string str && string.IsNullOrWhiteSpace(str)))
                RecordValidationError(ErrorCodes.ERR_ARGUMENT_NULL, $"{propertyName ?? "Property"} cannot be null.");
            return this;
        }

        IValidatorPropertyOptions<TModel> IValidatorPropertyOptions<TModel>.MinimumLength(int length)
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

        IValidatorPropertyOptions<TModel> IValidatorPropertyOptions<TModel>.MaximumLength(int length)
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

        IValidatorEntityOptions<TModel> IValidatorEntityOptions<TModel>.MustExist()
        {
            if (hasFailed()) return this;
            if (db == null) return RecordValidationError(ErrorCodes.ERR_ARGUMENT_NULL, "Database was not passed.", true);
            if (entity == null) return RecordValidationError(ErrorCodes.ERR_NOT_FOUND, "Entity not found.");
            return this;
        }

        IValidatorEntityOptions<TModel> IValidatorEntityOptions<TModel>.MustNotExist()
        {
            if (hasFailed()) return this;
            if (db == null) return RecordValidationError(ErrorCodes.ERR_ARGUMENT_NULL, "Database was not passed.", true);
            if (entity != null) return RecordValidationError(ErrorCodes.ERR_ENTITY_ALREADY_EXISTS, "Entity already exists.");
            return this;
        }

        bool ICanValidate<TModel>.Validate()
        {
            if (Model == null) return false;
            return !errors.Any();
        }

        Success<TResult> ICanValidate<TModel>.GetResult<TResult>()
        {
            var firstError = errors.FirstOrDefault();
            if (firstError != null)
                return new Success<TResult>(firstError.ErrorCode, firstError?.Message ?? "Unspecified error.");
            else
                return new Success<TResult>(true);
        }

        Success ICanValidate<TModel>.GetResult()
        {
            var firstError = errors.FirstOrDefault();

            if (firstError != null)
                return new Success(firstError.ErrorCode, firstError.Message ?? "Unspecified error.");
            else
                return new Success(true);
        }

        IValidatorValueOptions<TModel> IValidatorFirstStage<TModel>.Value(object value, string? valueName)
        {
            this.value = value;
            this.valueName = valueName;
            return this;
        }

        IValidatorValueOptions<TModel> IValidatorValueOptions<TModel>.NotNullOrWhiteSpace()
        {
            if (hasFailed()) return this;
            if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                RecordValidationError(ErrorCodes.ERR_ARGUMENT_NULL, $"{valueName ?? "Value"} cannot be null.");
            return this;
        }

        IValidatorPropertyOptions<TModel> IValidatorPropertyOptions<TModel>.MustBeGreaterThan(int minValue) => NumericMustBeGreaterThan(minValue);
        IValidatorPropertyOptions<TModel> IValidatorPropertyOptions<TModel>.MustBeLessThan(int maxValue) => NumericMustBeLessThan(maxValue);
        IValidatorPropertyOptions<TModel> IValidatorPropertyOptions<TModel>.MustEqual(int value) => NumericMustEqual(value);
        IValidatorPropertyOptions<TModel> IValidatorPropertyOptions<TModel>.MustBeGreaterThan(long minValue) => NumericMustBeGreaterThan(minValue);
        IValidatorPropertyOptions<TModel> IValidatorPropertyOptions<TModel>.MustBeLessThan(long maxValue) => NumericMustBeLessThan(maxValue);
        IValidatorPropertyOptions<TModel> IValidatorPropertyOptions<TModel>.MustEqual(long value) => NumericMustEqual(value);

        private IValidatorPropertyOptions<TModel> NumericMustEqual(object value)
        {
            if (hasFailed()) return this;
            if (property is int intVal && int.TryParse(value.ToString(), out int intTarget))
            {
                if (intVal != intTarget)
                    return RecordValidationError(ErrorCodes.ERR_VALUE_TOO_LONG, $"{propertyName ?? "Property"} must be equal to {value}.");
            }
            else if (property is long longVal && long.TryParse(value.ToString(), out long longTarget))
            {
                if (longVal != longTarget)
                    return RecordValidationError(ErrorCodes.ERR_VALUE_TOO_LONG, $"{propertyName ?? "Property"} must be equal to {value}.");
            }
            else if (decimal.TryParse(property?.ToString() ?? "x", out decimal decVal) && decimal.TryParse(value.ToString(), out decimal decTarget))
            {
                if (decVal != decTarget)
                    return RecordValidationError(ErrorCodes.ERR_VALUE_TOO_LONG, $"{propertyName ?? "Property"} must be equal to {value}.");
            }
            else
            {
                return RecordValidationError(ErrorCodes.ERR_TYPE_MISMATCH, $"Type mismatch: {propertyName ?? "Property"} is not a number.");
            }
            return this;
        }

        private IValidatorPropertyOptions<TModel> NumericMustBeGreaterThan(object value)
        {
            if (hasFailed()) return this;
            if (property is int intVal && int.TryParse(value.ToString(), out int intTarget))
            {
                if (intVal <= intTarget)
                    return RecordValidationError(ErrorCodes.ERR_VALUE_TOO_LONG, $"{propertyName ?? "Property"} must be greater than {value}.");
            }
            else if (property is long longVal && long.TryParse(value.ToString(), out long longTarget))
            {
                if (longVal <= longTarget)
                    return RecordValidationError(ErrorCodes.ERR_VALUE_TOO_LONG, $"{propertyName ?? "Property"} must be greater than  {value}.");
            }
            else if (decimal.TryParse(property?.ToString() ?? "x", out decimal decVal) && decimal.TryParse(value.ToString(), out decimal decTarget))
            {
                if (decVal <= decTarget)
                    return RecordValidationError(ErrorCodes.ERR_VALUE_TOO_LONG, $"{propertyName ?? "Property"} must be greater than  {value}.");
            }
            else
            {
                return RecordValidationError(ErrorCodes.ERR_TYPE_MISMATCH, $"Type mismatch: {propertyName ?? "Property"} is not a number.");
            }
            return this;
        }

        private IValidatorPropertyOptions<TModel> NumericMustBeLessThan(object value)
        {
            if (hasFailed()) return this;
            if (property is int intVal && int.TryParse(value.ToString(), out int intTarget))
            {
                if (intVal >= intTarget)
                    return RecordValidationError(ErrorCodes.ERR_VALUE_TOO_LONG, $"{propertyName ?? "Property"} must be less than {value}.");
            }
            else if (property is long longVal && long.TryParse(value.ToString(), out long longTarget))
            {
                if (longVal >= longTarget)
                    return RecordValidationError(ErrorCodes.ERR_VALUE_TOO_LONG, $"{propertyName ?? "Property"} must be less than  {value}.");
            }
            else if (decimal.TryParse(property?.ToString() ?? "x", out decimal decVal) && decimal.TryParse(value.ToString(), out decimal decTarget))
            {
                if (decVal >= decTarget)
                    return RecordValidationError(ErrorCodes.ERR_VALUE_TOO_LONG, $"{propertyName ?? "Property"} must be less than  {value}.");
            }
            else
            {
                return RecordValidationError(ErrorCodes.ERR_TYPE_MISMATCH, $"Type mismatch: {propertyName ?? "Property"} is not a number.");
            }
            return this;
        }

    }

}
