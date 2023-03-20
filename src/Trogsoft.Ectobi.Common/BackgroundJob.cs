using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.Common
{
    public class BackgroundJob : IBackgroundJob
    {
        public BackgroundJob(BackgroundJobInfo job) 
        {
            Job = job;
        }

        protected BackgroundJobInfo Job { get; }
    }
}
