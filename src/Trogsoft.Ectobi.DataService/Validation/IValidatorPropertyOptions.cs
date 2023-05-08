namespace Trogsoft.Ectobi.DataService.Validation
{
    public interface IValidatorPropertyOptions<T> : IValidatorFirstStage<T>, ICanValidate<T>
    {
        IValidatorPropertyOptions<T> NotNullOrWhiteSpace();
        IValidatorPropertyOptions<T> MinimumLength(int length);
        IValidatorPropertyOptions<T> MaximumLength(int length);
        IValidatorPropertyOptions<T> MustBeGreaterThan(int minValue);
        IValidatorPropertyOptions<T> MustBeLessThan(int maxValue);
        IValidatorPropertyOptions<T> MustEqual(int value);
        IValidatorPropertyOptions<T> MustBeGreaterThan(long minValue);
        IValidatorPropertyOptions<T> MustBeLessThan(long maxValue);
        IValidatorPropertyOptions<T> MustEqual(long value);
    }

}
