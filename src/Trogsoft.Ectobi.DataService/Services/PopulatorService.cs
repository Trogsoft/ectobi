using Newtonsoft.Json;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Interfaces;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class PopulatorService : IPopulatorService
    {
        private readonly ModuleManager mm;

        public PopulatorService(ModuleManager mm)
        {
            this.mm = mm;
        }

        public Value GetPopulatedValue(SchemaFieldVersion schemaFieldVersion)
        {

            if (schemaFieldVersion.Populator == null) throw new ArgumentNullException("schemaFieldVersion.Populator");

            var pop = mm.GetPopulator(schemaFieldVersion.Populator.TextId);
            if (pop == null) throw new PoulatorNotFoundException();

            var val = new Value();
            val.SchemaFieldVersionId = schemaFieldVersion.Id;

            var pconfig = schemaFieldVersion.PopulatorConfiguration;
            var poptions = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(pconfig))
                poptions = JsonConvert.DeserializeObject<Dictionary<string, string>>(pconfig);

            var rt = pop.GetReturnType();
            switch (rt)
            {
                case PopulatorReturnType.Integer:
                    val.IntegerValue = pop.GetInteger(poptions);
                    break;

                case PopulatorReturnType.Decimal:
                    val.DecimalValue = pop.GetDecimal(poptions);
                    break;

                case PopulatorReturnType.String:
                default:
                    val.RawValue = pop.GetString(poptions);
                    break;
            }

            return val;

        }

    }
}
