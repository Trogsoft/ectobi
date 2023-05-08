using System.Runtime.Serialization;

namespace Trogsoft.Ectobi.DataService
{
    [Serializable]
    internal class ModelNotFoundException : Exception
    {
        public ModelNotFoundException()
        {
        }

        public ModelNotFoundException(string? message) : base(message)
        {
        }

        public ModelNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ModelNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}