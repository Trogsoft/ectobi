using System.Runtime.Serialization;

namespace Trogsoft.Ectobi.DataService
{
    [Serializable]
    internal class PoulatorNotFoundException : Exception
    {
        public PoulatorNotFoundException()
        {
        }

        public PoulatorNotFoundException(string? message) : base(message)
        {
        }

        public PoulatorNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PoulatorNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}