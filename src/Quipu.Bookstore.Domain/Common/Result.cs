namespace Quipu.Bookstore.Domain.Common
{
    public class Result
    {
        public bool IsSuccess { get; }

        public string Error { get; }

        public string ErrorCode { get; }

        public ErrorType ErrorType { get; }

        protected Result(bool isSuccess, string error, string errorCode, ErrorType errorType)
        {
            if (isSuccess && (!string.IsNullOrEmpty(error) || !string.IsNullOrEmpty(errorCode) || errorType != ErrorType.None))
                throw new InvalidOperationException("A successful result cannot carry an error.");

            if (!isSuccess && (string.IsNullOrEmpty(error) || string.IsNullOrEmpty(errorCode) || errorType == ErrorType.None))
                throw new InvalidOperationException("A failed result must carry an error, an error code and an error type.");

            IsSuccess = isSuccess;
            Error = error;
            ErrorCode = errorCode;
            ErrorType = errorType;
        }

        public static Result Ok()
            => new(true, string.Empty, string.Empty, ErrorType.None);

        public static Result NotFound(string error, string errorCode)
            => new(false, error, errorCode, ErrorType.NotFound);

        public static Result Invalid(string error, string errorCode)
            => new(false, error, errorCode, ErrorType.Validation);
    }

    public class Result<T>(T? value, bool isSuccess, string error, string errorCode, ErrorType errorType)
        : Result(isSuccess, error, errorCode, errorType)
    {
        public T Value { get; } = value!;

        public static Result<T> Ok(T value)
            => new(value, true, string.Empty, string.Empty, ErrorType.None);

        public static new Result<T> NotFound(string error, string errorCode)
            => new(default, false, error, errorCode, ErrorType.NotFound);

        public static new Result<T> Invalid(string error, string errorCode)
            => new(default, false, error, errorCode, ErrorType.Validation);
    }
}
