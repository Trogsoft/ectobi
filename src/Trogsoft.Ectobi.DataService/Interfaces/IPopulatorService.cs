using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Interfaces
{
    public interface IPopulatorService
    {
        Value GetPopulatedValue(SchemaFieldVersion schemaFieldVersion);
    }
}
