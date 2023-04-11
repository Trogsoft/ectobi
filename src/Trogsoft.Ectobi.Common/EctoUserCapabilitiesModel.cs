namespace Trogsoft.Ectobi.Common
{
    public class EctoUserCapabilitiesModel
    {
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public Guid? UserId { get; set; }
        public bool CanManageSchemas { get; set; }
        public bool CanManageFields { get; set; }
        public bool CanManageBatches { get; set; }
        public bool CanInputData { get; set; }
        public bool CanManageUsers { get; set; }
    }
}