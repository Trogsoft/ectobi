using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IFileHandler
    {
        Success<List<string>> GetHeaders(string? sheetName = null);
        Success LoadFile(BinaryFileModel model);
    }
}
