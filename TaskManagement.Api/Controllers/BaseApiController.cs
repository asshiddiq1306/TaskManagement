using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult HandleServiceResult<T>(ServiceResult<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            if (result.ValidationErrors.Any())
            {
                foreach (var error in result.ValidationErrors)
                {
                    ModelState.AddModelError("", error);
                }
                return BadRequest(ModelState);
            }

            return BadRequest(result.ErrorMessage);
        }

        protected IActionResult HandleServiceResult(ServiceResult result)
        {
            if (result.IsSuccess)
            {
                return Ok();
            }

            if (result.ValidationErrors.Any())
            {
                foreach (var error in result.ValidationErrors)
                {
                    ModelState.AddModelError("", error);
                }
                return BadRequest(ModelState);
            }

            return BadRequest(result.ErrorMessage);
        }

        protected string GetCurrentUser()
        {
            // For now, return a default user. In a real app, this would come from JWT claims
            return "api-user";
        }
    }
}
