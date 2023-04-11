namespace Trogsoft.Ectobi.Data
{
    public class UserRefreshToken
    {
        public long Id { get; set; }
        public Guid UserId { get; set; }
        public EctoUser User { get; set; }
        public string Token { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public string SessionIdentifier { get; set; }
    }
}