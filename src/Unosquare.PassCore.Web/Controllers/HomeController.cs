namespace Unosquare.PassCore.Web.Controllers
{
    using System.IO;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// This controller is simply a placeholder to redirect any non-matching URL
    /// to provide the context of the SPA (single page application) index
    /// Examine the routing configuration in the Startup class
    /// </summary>
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            // return Json(new { result = "Hello World!!!!" });
            try{
                return File("~/index.html", "text/html");
            }catch(FileNotFoundException ex){
                return Json(new { result = ex.Message });
            }
        }
    }
}
