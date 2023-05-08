namespace Trogsoft.Ectobi.DataService.Validation
{
    public interface IValidatorValueOptions<T> : IValidatorFirstStage<T>, ICanValidate<T>
    {
        IValidatorValueOptions<T> NotNullOrWhiteSpace();
    }
}