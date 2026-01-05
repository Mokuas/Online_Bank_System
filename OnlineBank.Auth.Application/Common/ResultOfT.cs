namespace OnlineBank.Auth.Application.Common
{
    /// <summary>
    /// Generic result type used for returning either a value on success or an error message on failure.
    /// This avoids exception-driven flow for expected cases (invalid credentials),
    /// keeps controller/service logic explicit, and avoids adding an external dependency for a small utility.
    /// </summary>
    public sealed class Result<T>
    {
        public bool IsSuccess { get; }
        public string? Error { get; }
        public T? Value { get; }

        private Result(bool isSuccess, T? value, string? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new(true, value, null);
        public static Result<T> Failure(string error) => new(false, default, error);
    }
}