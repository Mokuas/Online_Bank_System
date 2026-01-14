namespace ProfilesService.Application.Common
{
    /// <summary>
    /// Lightweight result type used to represent success/failure without throwing exceptions for expected business outcomes.
    /// We keep this custom (instead of introducing a dependency) to keep the Application layer small, explicit, and stable.
    /// </summary>
    public sealed class Result
    {
        public bool IsSuccess { get; }
        public Error? Error { get; }

        private Result(bool isSuccess, Error? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);

        public static Result Failure(Error error) => new(false, error);
    }
}
