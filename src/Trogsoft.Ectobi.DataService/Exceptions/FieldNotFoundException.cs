using System.Runtime.Serialization;

namespace Trogsoft.Ectobi.DataService
{
    [Serializable]
    internal class FieldNotFoundException : Exception
    {
        public FieldNotFoundException()
        {
        }

        public FieldNotFoundException(string? message) : base(message)
        {
        }

        public FieldNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected FieldNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}