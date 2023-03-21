﻿using Microsoft.AspNetCore.Mvc;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Controllers
{
    [Route("api/field")]
    public class FieldController : EctoApiController
    {
        private readonly IFieldService fields;

        public FieldController(IFieldService fields)
        {
            this.fields = fields;
        }

        [HttpGet, Route("{schemaTid}")]
        public async Task<IActionResult> GetFields(string schemaTid) => SuccessResponse(await fields.GetFields(schemaTid));

        [HttpPost, Route("{schemaTid}")]
        public async Task<IActionResult> CreateField(string schemaTid, [FromBody] SchemaFieldEditModel model)
            => SuccessResponse(await fields.CreateField(schemaTid, model));

        [HttpDelete, Route("{schemaTid}/{fieldTid}")]
        public async Task<IActionResult> DeleteField(string schemaTid, string fieldTid) => SuccessResponse(await fields.DeleteField(schemaTid, fieldTid));

    }
}