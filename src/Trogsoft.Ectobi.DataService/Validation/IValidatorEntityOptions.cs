namespace Trogsoft.Ectobi.DataService.Validation
{
    public interface IValidatorEntityOptions<T> : IValidatorFirstStage<T>, ICanValidate<T>
    {
        IValidatorEntityOptions<T> MustExist();
        IValidatorEntityOptions<T> MustNotExist();
    }

}
