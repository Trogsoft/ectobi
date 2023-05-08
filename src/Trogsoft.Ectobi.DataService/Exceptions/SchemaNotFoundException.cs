using System.Runtime.Serialization;

namespace Trogsoft.Ectobi.DataService
{
    [Serializable]
    internal class SchemaNotFoundException : Exception
    {
        public SchemaNotFoundException()
        {
        }

        public SchemaNotFoundException(string? message) : base(message)
        {
        }

        public SchemaNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SchemaNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}