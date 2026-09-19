using Quipu.Bookstore.Domain.Common;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Quipu.Bookstore.Api.Extensions
{
    public static class ResultExtensions
    {
        private const string ErrorCodeExtension = "errorCode";

        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
                return new NoContentResult();

            return result.ToProblemResult();
        }

        public static IActionResult ToActionResult<T>(this Result<T> result, Func<T, IActionResult> onSuccess)
        {
            if (result.IsSuccess)
                return onSuccess(result.Value);

            return result.ToProblemResult();
        }

        private static ObjectResult ToProblemResult(this Result result)
        {
            (int statusCode, string type) = result.ErrorType switch
            {
                ErrorType.Validation => (StatusCodes.Status400BadRequest, "https://tools.ietf.org/html/rfc9110#section-15.5.1"),
                ErrorType.NotFound => (StatusCodes.Status404NotFound, "https://tools.ietf.org/html/rfc9110#section-15.5.5"),
                _ => throw new InvalidOperationException($"Error type '{result.ErrorType}' has no HTTP status mapping.")
            };

            ProblemDetails problem = new()
            {
                Type = type,
                Title = ReasonPhrases.GetReasonPhrase(statusCode),
                Status = statusCode,
                Detail = result.Error
            };

            problem.Extensions[ErrorCodeExtension] = result.ErrorCode;

            return new ObjectResult(problem) { StatusCode = statusCode };
        }
    }
}
