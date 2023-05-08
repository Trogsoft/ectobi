using System.Runtime.Serialization;

namespace Trogsoft.Ectobi.DataService
{
    [Serializable]
    internal class LookupSetNotFoundException : Exception
    {
        public LookupSetNotFoundException()
        {
        }

        public LookupSetNotFoundException(string? message) : base(message)
        {
        }

        public LookupSetNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected LookupSetNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}