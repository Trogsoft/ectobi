using ClosedXML.Excel;
using DocumentFormat.OpenXml.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.CommandLine;
using Trogsoft.Ectobi.Client;
using Trogsoft.Ectobi.Common;

namespace ectobi
{
    [Verb("schema")]
    public class Schema : EctoVerb
    {

        [Operation("create")]
        [Parameter('f', "file", IsRequired = true, HelpText = "Specify a file the schema should be read from (json, xlsx)")]
        [Parameter('n', "name", IsRequired = true, HelpText = "The name of the schema")]
        [Parameter('o', "overwrite", IsRequired = false, HelpText = "Overwrite an existing schema of the same name")]
        [Parameter("verbose", IsRequired = false, HelpText = "Output more details")]
        public int Create(string file, string name, bool overwrite = false, bool verbose = false)
        {

            if (!File.Exists(file))
            {
                WriteError("File not found: " + file);
                return 10;
            }

            var model = new SchemaEditModel();
            model.Overwrite = overwrite;
            model.Name = name;

            if (file.EndsWith(".xlsx", StringComparison.CurrentCultureIgnoreCase))
            {
                CreateModelFromExcelFile(model, file);
            }

            var result = InstanceManager.Client.Schema.CreateSchemaAsync(model).Result;
            return WriteSuccess(result, verbose);

        }

        [Operation("delete")]
        [Parameter("id", IsRequired = true, HelpText = "The text identified for the schema to delete.")]
        public int Delete(string textid)
        {

            var result = InstanceManager.Client.Schema.DeleteSchemaSync(textid).Result;
            return WriteSuccess(result);

        }

        [Operation("list")]
        public int List()
        {
            var result = InstanceManager.Client.Schema.GetSchemas(false).Result;
            if (result.Result != null)
                result.Result.ForEach(x => Console.WriteLine($"{x.TextId} ({x.Name})"));
            else
                WriteWarning("No items.");
            return 0;
        }

        private void CreateModelFromExcelFile(SchemaEditModel model, string file)
        {
            var xl = new XLWorkbook(file);
            var ws = xl.Worksheets.FirstOrDefault();

            var columnCount = ws.ColumnsUsed().Count();
            for (var x = 1; x < columnCount; x++)
            {

                var value = ws.Cell(1, x).GetValue<string>();
                if (value == null) break;

                var field = new SchemaFieldEditModel
                {
                    Name = value
                };

                field.RawValues = GetColumnValues(ws, x);

                model.Fields.Add(field);

            }

        }

        private List<string> GetColumnValues(IXLWorksheet ws, int x)
        {

            List<string> list = new List<string>();
            var rowCount = ws.RowsUsed().Count();
            for (var y = 2; y < rowCount; y++)
            {
                var value = ws.Cell(y, x).GetValue<string>();
                if (value != null)
                    list.Add(value);
            }
            return list;

        }
    }
}
