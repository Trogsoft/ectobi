using System.Linq.Expressions;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Validation
{
    public interface IValidatorFirstStage<T> : ICanValidate<T>
    {
        IValidatorPropertyOptions<T> Property<TProp>(Expression<Func<T, TProp>> selector);
        IValidatorEntityOptions<T> Entity<TEntity>(Func<T, string> selector) where TEntity : NamedEntity;
        IValidatorEntityOptions<T> Entity<TEntity>(Func<TEntity, bool> selector) where TEntity : class;
        IValidatorValueOptions<T> Value(object value, string? valueName);
    }

}
