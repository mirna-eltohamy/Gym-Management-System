using GymManagementSystem.BLL.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL
{
    public record Result(bool success, string? error = null, ResultKind kind = ResultKind.OK)
    {
        public static Result OK() => new(true);

        public static Result Fail(string message, ResultKind kind = ResultKind.Conflict) => new(false, message, kind);

        public static Result NotFound(string message = "Not Found") => new(false, message, ResultKind.NotFound);

        public static Result Validation(string message ) => new(false, message, ResultKind.Validation);
    }

    public record Result<T>(bool success, T? value, string? error = null, ResultKind kind = ResultKind.OK)
    {
        public static Result<T> OK(T value) => new(true, value);

        public static Result<T> Fail(string message, ResultKind kind = ResultKind.Conflict) => new(false, default, message, kind);

        public static Result<T> NotFound(string message = "Not Found") => new(false, default, message, ResultKind.NotFound);

        public static Result<T> Validation(string message) => new(false, default, message, ResultKind.Validation);
    }
}
