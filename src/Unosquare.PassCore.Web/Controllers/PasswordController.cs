namespace Unosquare.PassCore.Web.Controllers
{
    using Microsoft.AspNet.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.Net;
    using System.Threading.Tasks;
    using Unosquare.PassCore.Web.Models;

    [Route("api/[controller]")]
    public class PasswordController : ControllerBase
    {
        public PasswordController(IConfigurationRoot configuration)
            : base(configuration)
        {
            // placeholder
        }

        // POST api/password
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ChangePasswordModel value)
        {

            if (value == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ApiResult.InvalidRequest());
            }

            var result = new ApiResult();
            if (ModelState.IsValid == false)
            {
                result.AddModelStateErrors(ModelState);
            }
            else
            {
                await Task.Delay(2000);
                // TODO: Perform the password change here
            }

            if (result.HasErrors)
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json(result);

        }
    }
}
