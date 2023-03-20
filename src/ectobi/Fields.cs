using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.CommandLine;
using Trogsoft.Ectobi.Common;

namespace ectobi
{
    [Verb("field")]
    public class Field : EctoVerb
    {
        [Operation("list")]
        [Parameter('s', "schema", Position = 0, IsRequired = true)]
        public int List(string schema)
        {
            var result = InstanceManager.Client.Fields.GetFields(schema).Result;
            if (result.Result != null)
                result.Result.ForEach(x => Console.WriteLine($"{x.TextId} ({x.Name})"));
            else
                WriteWarning("No items.");
            return 0;
        }

        [Operation("create")]
        public int Create(FieldCreateModel model)
        {
            var sfeModel = new SchemaFieldEditModel
            {
                Name = model.Name,
                Description = model.Description,
                Type = model.Type,
                Flags = model.Flags
            };
            var result = InstanceManager.Client.Fields.CreateField(model.SchemaTid, sfeModel).Result;
            if (result.Succeeded)
            {
                WriteSuccess("Success.");
            }
            else
            {
                WriteError($"Failed: {result.StatusMessage}");
                return result.ErrorCode;
            }
            return 0;
        }
    }
}
