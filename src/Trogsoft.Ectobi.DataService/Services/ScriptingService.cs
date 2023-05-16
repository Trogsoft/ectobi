using DocumentFormat.OpenXml.Vml;
using Jint;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Interfaces;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class ScriptingService : IScriptingService, IScriptingFormulaResult
    {

        object? resultObject = null;

        public ScriptingService() 
        {
        }

        public IScriptingFormulaResult ExecuteFormula(string formula, Dictionary<string, object> values) 
        {
            var je = new Engine(opts =>
            {
                opts.Strict();
                opts.TimeoutInterval(TimeSpan.FromMilliseconds(50));
            });

            foreach (var key in values.Keys)
            {
                je.SetValue(key, values[key]);
            }

            resultObject = je.Evaluate(formula).ToObject();
            return this;
        }

        public object? GetObject() => this.resultObject;

        public IValueObject GetRecordValue(IValueObject? value = null)
        {
            if (value == null)
                value = new Value();

            if (resultObject is int intValue) value.IntegerValue = (long)intValue;
            else if (resultObject is long longValue) value.IntegerValue = longValue;
            else if (resultObject is double doubleValue) value.DecimalValue = doubleValue;
            else if (resultObject is float floatValue) value.DecimalValue = floatValue;
            else if (resultObject is bool boolValue) value.BoolValue = boolValue;
            else value.RawValue = resultObject.ToString();

            return value;
        }

    }
}
