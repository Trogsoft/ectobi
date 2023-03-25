using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.CommandLine;
using Trogsoft.Ectobi.Common;

namespace ectobi
{
    [Verb("batch")]
    public class Batch : EctoVerb
    {

        [Operation("import")]
        public int Import(string file, string schema, string? name = null)
        {
            if (!File.Exists(file))
            {
                WriteError($"File not found: {file}");
                return ErrorCodes.ERR_NOT_FOUND;
            }

            Stopwatch sw = Stopwatch.StartNew();
            Console.Write("Reading file...");
            var model = CreateModelFromExcelFile(file, schema, name);
            sw.Stop();
            var ts = TimeSpan.FromTicks(sw.ElapsedTicks);
            Console.WriteLine($" took {ts}.");

            var result = InstanceManager.Client.Batches.ImportBatch(model).Result;
            if (result.Succeeded)
                WriteSuccess(result);
            else
            {
                WriteError(result.StatusMessage);
                return result.ErrorCode;
            }

            return 0;
        }

        private ImportBatchModel CreateModelFromExcelFile(string file, string schema, string? name = null)
        {
            FileInfo fi = new FileInfo(file);

            var model = new ImportBatchModel
            {
                BatchName = name ?? Path.GetFileNameWithoutExtension(fi.Name),
                SchemaTid = schema,
                BatchSource = file
            };

            var xl = new XLWorkbook(file);
            var ws = xl.Worksheets.FirstOrDefault();
            if (ws == null)
                throw new Exception("No worksheets.");

            for (var x = 1; x <= ws.ColumnsUsed().Count(); x++)
            {
                var header = ws.Cell(1, x).GetValue<string>();
                model.ValueMap.Headings.Add(header);
            }

            for (var y = 2; y <= ws.RowsUsed().Count(); y++)
            {
                var row = new ValueMapRow();
                for (var x = 1; x <= ws.ColumnsUsed().Count(); x++)
                {
                    row.Add(ws.Cell(y, x).GetValue<string?>()!);
                }
                model.ValueMap.Rows.Add(row);
            }

            return model;
        }
    }
}
