using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class WebHookEventModel
    {
        public WebHookEventModel()
        {
        }

        public DateTime Raised { get; set; } = DateTime.Now;

    }

    public class WebHookEventModel<T> : WebHookEventModel
    {
        public WebHookEventModel(T data) : base()
        {
            Data = data;
        }

        public T Data { get; }
    }

    public class WebHookSchemaEvent : WebHookEventModel
    {
        public WebHookSchemaEvent(string schemaTid, int version = 0)
        {
            SchemaTid = schemaTid;
            Version = version;
        }

        public int Version { get; set; }
        public string SchemaTid { get; }
    }

    public class WebHookFieldEvent : WebHookSchemaEvent
    {
        public WebHookFieldEvent(string schemaTid, string fieldTid) : base(schemaTid)
        {
            FieldTid = fieldTid;
            IsRootField = true;
        }

        public WebHookFieldEvent(string schemaTid, int version, string fieldTid) : base(schemaTid, version)
        {
            FieldTid = fieldTid;
            IsRootField = false;
        }

        public bool IsRootField { get; set; }
        public string FieldTid { get; }
    }

}
