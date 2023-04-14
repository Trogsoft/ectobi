namespace Trogsoft.Ectobi.Common
{
    [Flags]
    public enum EctoModelPropertyFlags
    {
        None = 0,
        PersonallyIdentifiableInformation = 1,
        GeographicIdentifier = 2,
        EmailAddress = 4,
        PhoneNumber = 8,
        SuggestedDefault = 16
    }
}