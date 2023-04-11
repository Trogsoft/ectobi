using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Controllers
{
    [Route("api/schema"), Authorize]
    public class SchemaController : EctoApiController
    {
        private readonly ISchemaService schema;

        public SchemaController(ISchemaService schema)
        {
            this.schema = schema;
        }

        [HttpPost, Authorize(Roles ="Administrator,SchemaManager")]
        public async Task<IActionResult> CreateSchema([FromBody] SchemaEditModel model) => SuccessResponse(await schema.CreateSchema(model));

        [HttpDelete("{schemaTid}"), Authorize(Roles = "Administrator,SchemaManager")]
        public async Task<IActionResult> DeleteSchema(string schemaTid) => SuccessResponse(await schema.DeleteSchema(schemaTid));

        [HttpGet, Authorize(Roles = "Administrator,SchemaManager")]
        public async Task<IActionResult> ListSchemas(bool includeDetail = false) => SuccessResponse(await schema.GetSchemas(includeDetail));

        [HttpGet("{textId}"), Authorize(Roles = "Administrator,SchemaManager")]
        public async Task<IActionResult> GetSchema(string textId) => SuccessResponse(await schema.GetSchema(textId));

        // Versions

        [HttpGet("{schemaTid}/versions"), Authorize(Roles = "Administrator,SchemaManager")]
        public async Task<IActionResult> GetSchemaVersions(string schemaTid) => SuccessResponse(await schema.GetSchemaVersions(schemaTid));

        [HttpPost("{schemaTid}/versions"), Authorize(Roles = "Administrator,SchemaManager")]
        public async Task<IActionResult> CreateSchemaVersion([FromBody] SchemaVersionEditModel model) => SuccessResponse(await schema.CreateSchemaVersion(model));

    }
}
