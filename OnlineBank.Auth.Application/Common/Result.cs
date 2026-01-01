using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBank.Auth.Application.Common
{
    public sealed class Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }

        private Result(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);

        public static Result Failure(string error) => new(false, error);
    }
}
