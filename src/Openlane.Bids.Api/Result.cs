namespace Openlane.Bids.Api
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public ErrorType? ErrorType { get; }
        public string? ErrorMessage { get; }

        private Result(T value)
        {
            IsSuccess = true;
            Value = value;
        }

        private Result(ErrorType errorType, string errorMessage)
        {
            IsSuccess = false;
            ErrorType = errorType;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(ErrorType errorType, string errorMessage) => new(errorType, errorMessage);
    }

    public enum ErrorType
    {
        ValidationError,
        NotFoundError,
        InternalServerError
    }
}
