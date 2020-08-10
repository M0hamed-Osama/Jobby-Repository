using Jobby.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jobby.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ViewResult Index()
        {
            ViewBag.UserType = SecurityUtilities.GetAuthenticatedUserType();
            return View("Error");
        }
        public ViewResult NotFound()
        {
            ViewBag.UserType = SecurityUtilities.GetAuthenticatedUserType();
            Response.StatusCode = 404;  //you may want to set this to 200
            return View("NotFound");
        }
    }
}