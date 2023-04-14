﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IDataService
    {
        Task<Success<ValueMap>> GetData(DataQueryModel query);
    }
}