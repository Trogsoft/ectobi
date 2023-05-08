using System.Runtime.Serialization;

namespace Trogsoft.Ectobi.DataService
{
    [Serializable]
    internal class SchemaVersionNotFoundException : Exception
    {
        public SchemaVersionNotFoundException()
        {
        }

        public SchemaVersionNotFoundException(string? message) : base(message)
        {
        }

        public SchemaVersionNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SchemaVersionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}