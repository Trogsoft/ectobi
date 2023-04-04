using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Presentation.Command;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.FileHandlers
{
    [FileHandler("Excel", Extensions = new string[] { "xlsx" })]
    public class ExcelFileImporter : IFileHandler
    {

        XLWorkbook? xlb = null;

        public ExcelFileImporter() { 
        }

        public Success LoadFile(BinaryFileModel model)
        {
            if (model == null) return Success.Error("Model cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);
            if (string.IsNullOrWhiteSpace(model.Filename)) return Success.Error("Model.Filename cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);
            if (model.Data == null) return Success.Error("Model.Data cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);

            MemoryStream ms = new MemoryStream(model.Data);
            this.xlb = new XLWorkbook(ms);

            return new Success(true);
        }

        public Success<List<string>> GetHeaders(string? sheetName = null)
        {

            if (xlb == null) return Success<List<string>>.Error("File is not loaded. Call LoadFile() first.", ErrorCodes.ERR_FILE_NOT_LOADED);

            var ws = xlb.Worksheets.FirstOrDefault();
            if (ws == null) return Success<List<string>>.Error("Files contains no worksheets.", ErrorCodes.ERR_FILE_PROCESSING_PROBLEM);

            var list = new List<string>();

            var columnCount = ws.ColumnsUsed().Count();
            if (columnCount == 0) return Success<List<string>>.Error("Files contains no data.", ErrorCodes.ERR_FILE_PROCESSING_PROBLEM);
            for (var x = 1; x <= columnCount; x++)
            {

                var value = ws.Cell(1, x).GetValue<string>();
                if (value == null) break;

                list.Add(value);

            }

            return new Success<List<string>>(list);

        }

        public Success<List<string>> GetContentsOfColumn(string columnName, string? sheetName = null)
        {

            if (xlb == null) return Success<List<string>>.Error("File is not loaded. Call LoadFile() first.", ErrorCodes.ERR_FILE_NOT_LOADED);
            if (columnName == null) return Success<List<string>>.Error("ColumnName cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);

            var ws = xlb.Worksheets.FirstOrDefault();
            if (ws == null) return Success<List<string>>.Error("Files contains no worksheets.", ErrorCodes.ERR_FILE_PROCESSING_PROBLEM);

            var columnCount = ws.ColumnsUsed().Count();
            if (columnCount == 0) return Success<List<string>>.Error("Files contains no data.", ErrorCodes.ERR_FILE_PROCESSING_PROBLEM);

            int columnIndex = -1;
            for (var x = 1; x <= columnCount; x++)
            {
                var value = ws.Cell(1, x).GetValue<string>();
                if (value == null) break;
                if (value.Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                {
                    columnIndex = x;
                    break;
                }
            }

            List<string> result = new List<string>();

            var rowCount = ws.RowsUsed().Count();
            if (rowCount < 2) return Success<List<string>>.Error("File contains no data.", ErrorCodes.ERR_FILE_PROCESSING_PROBLEM);

            for (var y = 2; y < rowCount; y++)
            {
                var value = ws.Cell(y, columnIndex).GetValue<string>();
                result.Add(value);
            }

            return new Success<List<string>>(result);

        }

    }
}
