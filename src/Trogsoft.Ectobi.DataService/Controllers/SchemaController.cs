using Microsoft.AspNetCore.Mvc;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Controllers
{
    [Route("api/schema")]
    public class SchemaController : EctoApiController
    {
        private readonly ISchemaService schema;

        public SchemaController(ISchemaService schema)
        {
            this.schema = schema;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchema([FromBody] SchemaEditModel model) => SuccessResponse(await schema.CreateSchema(model));

        [HttpDelete("{schemaTid}")]
        public async Task<IActionResult> DeleteSchema(string schemaTid) => SuccessResponse(await schema.DeleteSchema(schemaTid));

        [HttpGet]
        public async Task<IActionResult> ListSchemas(bool includeDetail = false) => SuccessResponse(await schema.GetSchemas(includeDetail));

    }
}
