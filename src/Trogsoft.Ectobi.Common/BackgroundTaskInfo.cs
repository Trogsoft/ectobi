using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class BackgroundTaskInfo
    {
        public BackgroundTaskInfo() { }
        public BackgroundTaskInfo(string name)
        {
            this.Name = name;
        }
        public BackgroundTaskInfo(string name, Guid userId) : this(name)
        {
            this.UserId = userId;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public string? Name { get; set; }
    }
}
