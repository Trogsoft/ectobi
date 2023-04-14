using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.EctoModels
{
    [EctoModel("person", "Person")]
    public class PersonEctoModel : EctoModelBase<PersonModel>, IEctoModel<PersonModel>
    {
        public PersonEctoModel()
        {
        }

        // Typed interface methods ============================================================

        public override async Task<Success<PersonModel>> GetPopulatedModel()
        {
            var record = new PersonModel();
            return new Success<PersonModel>(record);
        }

    }
}
