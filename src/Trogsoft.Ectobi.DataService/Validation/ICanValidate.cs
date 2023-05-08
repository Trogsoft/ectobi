using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.DataService.Validation
{
    public interface ICanValidate<T>
    {
        bool Validate();
        Success<TResult> GetResult<TResult>();
        Success GetResult();
    }

}
