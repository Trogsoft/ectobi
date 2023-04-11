using Microsoft.Extensions.Options;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class EctoServer : IEctoServer
    {
        private readonly IOptions<EctoServerModel> esm;
        private readonly IHttpContextAccessor http;

        public EctoServer(IOptions<EctoServerModel> esm, IHttpContextAccessor http)
        {
            this.esm = esm;
            this.http = http;
        }

        private bool UserIsInAnyRole(params string[] roles)
        {
            if (http.HttpContext == null) throw new Exception("HttpContext cannot be null here.");
            foreach (var role in roles)
            {
                if (http.HttpContext.User.IsInRole(role)) return true;
            }
            return false;
        }

        public async Task<Success<EctoServerModel>> GetServerModel()
        {
            var model = new EctoServerModel
            {
                Name = esm.Value.Name,
                RequiresLogin = esm.Value.RequiresLogin,
            };

            if (model.RequiresLogin && http.HttpContext != null)
            {
                model.UserCapabilities = new EctoUserCapabilitiesModel
                {
                    Username = http.HttpContext?.User?.Identity?.Name,
                    CanManageUsers = UserIsInAnyRole("Administrator"),
                    CanInputData = UserIsInAnyRole("Administrator", "DataEntryOperator"),
                    CanManageBatches = UserIsInAnyRole("Administrator", "BatchManager"),
                    CanManageFields = UserIsInAnyRole("Administrator", "FieldManager"),
                    CanManageSchemas = UserIsInAnyRole("Administrator", "SchemaManager")
                };
            }

            return new Success<EctoServerModel>(model);
        }
    }
}
