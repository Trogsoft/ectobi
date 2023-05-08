using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Controllers
{
    [Route("api/field")]
    [Authorize(Roles = "Administrator,FieldManager")]
    public class FieldController : EctoApiController
    {
        private readonly IFieldService fields;

        public FieldController(IFieldService fields)
        {
            this.fields = fields;
        }

        [HttpGet, Route("{schemaTid}/{fieldTid}")]
        public async Task<IActionResult> GetFields(string schemaTid, string fieldTid) => SuccessResponse(await fields.GetField(schemaTid, fieldTid));

        [HttpGet, Route("{schemaTid}")]
        public async Task<IActionResult> GetFields(string schemaTid) => SuccessResponse(await fields.GetFields(schemaTid));

        [HttpGet, Route("{schemaTid}/version/{version}")]
        public async Task<IActionResult> GetVersionFields(string schemaTid, int version) 
            => SuccessResponse(await fields.GetVersionFields(schemaTid, version));

        [HttpPost, Route("{schemaTid}")]
        public async Task<IActionResult> CreateField(string schemaTid, [FromBody] SchemaFieldEditModel model)
            => SuccessResponse(await fields.CreateField(schemaTid, model));

        [HttpDelete, Route("{schemaTid}/{fieldTid}")]
        public async Task<IActionResult> DeleteField(string schemaTid, string fieldTid) => SuccessResponse(await fields.DeleteField(schemaTid, fieldTid));

        [HttpPut, Route("{schemaTid}")]
        public async Task<IActionResult> EditField(string schemaTid, [FromBody] SchemaFieldEditModel model)
            => SuccessResponse(await fields.UpdateField(schemaTid, model));

    }
}
