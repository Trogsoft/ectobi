using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IFieldService
    {
        void ApplyLookupValuesToField(string schemaTid, string? textId);

        /// <summary>
        /// Create a field in the latest version of the specified schema, and also in the root schema definition if 
        /// it doesn't already exist.
        /// </summary>
        /// <param name="schemaTid">The schema TextId</param>
        /// <param name="model">A model which defines the field to create</param>
        /// <returns>A <see cref="Success"/> object containing a <see cref="SchemaFieldModel"/> on success.</returns>
        Task<Success<SchemaFieldModel>> CreateField(string schemaTid, SchemaFieldEditModel model);

        /// <summary>
        /// Delete a field from the latest version of the schema (or the specified version)
        /// </summary>
        /// <param name="schemaTid">The schema TextId</param>
        /// <param name="fieldName">The field TextId</param>
        /// <param name="version">The version. Defaults to zero, which means the latest version.</param>
        /// <remarks>
        /// This method will delete the specified field from the specified version of the schema, but 
        /// in the event that the field does not exist in any other schema versions, the root field
        /// will also be deleted.
        /// </remarks>
        /// <returns>A <see cref="Success"/> object indicating success or failure.</returns>
        Task<Success> DeleteField(string schemaTid, string fieldName, int version = 0);

        /// <summary>
        /// Get the specified field from the latest version of the specified schema
        /// </summary>
        /// <param name="schemaTid">The schema TextId</param>
        /// <param name="fieldTid">The field TextId</param>
        /// <returns>A <see cref="Success"/> containing a <see cref="SchemaFieldEditModel"/> on success.</returns>
        Task<Success<SchemaFieldEditModel>> GetField(string schemaTid, string fieldTid);
        Task<Success<List<SchemaFieldModel>>> GetFields(string schemaTid);
        Task<Success<List<SchemaFieldModel>>> GetVersionFields(string schemaTid, int version = 0);
        Task<Success<SchemaFieldModel>> UpdateField(string schemaTid, SchemaFieldEditModel model);
    }
}
