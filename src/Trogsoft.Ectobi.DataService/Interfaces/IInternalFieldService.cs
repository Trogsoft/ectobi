using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Interfaces
{
    public interface IFieldBackgroundService : IFieldService
    {
        /// <summary>
        /// Populate a field
        /// </summary>
        /// <param name="job">Background job information</param>
        /// <param name="rootFieldId">The ID of the root field (NOT the versioned field)</param>
        void PopulateField(BackgroundTaskInfo job, long rootFieldId);
        void AutoDetectSchemaFieldParameters(SchemaFieldEditModel x);
        Success DeleteField(BackgroundTaskInfo job, string schemaTid, string fieldTid, int version = 0);
    }
}
