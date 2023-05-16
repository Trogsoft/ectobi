namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IScriptingFormulaResult
    {
        object? GetObject();
        IValueObject GetRecordValue(IValueObject? value = null);
    }
}
