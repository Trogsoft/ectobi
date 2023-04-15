namespace Trogsoft.Ectobi.Common
{
    [Flags]
    public enum SchemaFieldFlags
    {
        None = 0,
        AllowNull = 1,
        NumericID = 2,
        DisplayValue = 4,
        RequiredAtImport = 8,
        PersonallyIdentifiableInformation = 16
    }
}