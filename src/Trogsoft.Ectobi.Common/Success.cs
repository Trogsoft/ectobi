namespace Trogsoft.Ectobi.Common
{
    public class Success
    {

        public static Success Error(string message, int? errorCode = null)
        {
            if (errorCode.HasValue)
                return new Success(errorCode.Value, message);
            else
                return new Success(false, message);
        }

        public Success()
        {
        }

        public Success(bool succeeded)
        {
            Succeeded = succeeded;
        }

        public Success(bool succeeded, string message)
        {
            Succeeded = succeeded;
            StatusMessage = message;
        }

        public Success(int errorCode, string message)
        {
            this.Succeeded = false;
            this.ErrorCode = errorCode;
            this.StatusMessage = message;
        }

        public int ErrorCode { get; set; }
        public bool Succeeded { get; set; } = false;
        public string? StatusMessage { get; set; }
    }

    public class Success<TResult> : Success
    {

        public static Success<TResult> Error(string message, int? errorCode = null)
        {
            if (errorCode.HasValue)
                return new Success<TResult>(errorCode.Value, message);
            else
                return new Success<TResult>(false, message);
        }
        public Success()
        {
        }

        public Success(bool succeeded) : base(succeeded)
        {
        }

        public Success(bool succeeded, string message) : base(succeeded, message)
        {
        }

        public Success(TResult result) : base(true)
        {
            this.Result = result;
        }

        public Success(int errorCode, string message) : base(errorCode, message)
        {
        }

        public TResult? Result { get; set; } = default;

    }

}