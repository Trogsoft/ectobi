namespace Trogsoft.Ectobi.DataService.Validation
{
    public interface ICanAddModel<T>
    {
        IValidatorFirstStage<T> WithModel(T model);
    }

}
