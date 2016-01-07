using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Unosquare.PassCore.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var model = new Models.ChangePasswordModel();
            return View(model);
        }

        //
        // POST: /Home/Index

        [HttpPost]
        public ActionResult Index(Models.ChangePasswordModel model)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return View(model);


                var principalContext = new PrincipalContext(ContextType.Domain);
                var userPrincipal = UserPrincipal.FindByIdentity(principalContext, model.UserPrincipalName);

                if (userPrincipal == null) throw new Exception("User principal was not found");

                userPrincipal.ChangePassword(model.CurrentPassword, model.NewPassword);
                userPrincipal.Save();

                return RedirectToAction("Index", new { msg = 1 });
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("ActiveDirectory", ex.Message);
                if (model == null) model = new Models.ChangePasswordModel();
                return View(model);
            }
        }

    }
}
