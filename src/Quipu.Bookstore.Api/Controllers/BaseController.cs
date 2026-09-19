using Quipu.Bookstore.Api.Extensions;
using Quipu.Bookstore.Domain.Common;

using Microsoft.AspNetCore.Mvc;

namespace Quipu.Bookstore.Api.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult Result(Result result)
        {
            return result.ToActionResult();
        }

        protected IActionResult Result<T>(Result<T> result, Func<T, IActionResult> onSuccess)
        {
            return result.ToActionResult(onSuccess);
        }
    }
}
