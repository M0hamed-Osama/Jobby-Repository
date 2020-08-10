using Jobby.Models;
using Jobby.Utilities;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Jobby.Controllers
{
    [AllowAnonymous]
    public class EmployeeController : Controller
    {
        private JobbyEntities db = new JobbyEntities();

        public ActionResult Index()
        {
            //if account is already login, redirect to home page 
            if (Request.IsAuthenticated)
            {
                string controller = SecurityUtilities.GetAuthenticatedUserType();
                if (controller!="Employee")
                return RedirectToAction("Index", controller);
            }
            return View();
        }

        [OutputCache(Duration = 1, VaryByParam = "*")]
        public PartialViewResult LatestJobs()
        {
            var jobs = db.Jobs.OrderByDescending(j => j.PostDate).Take(6).ToList();
            return PartialView("_LatestJobs", jobs);
        }

        [OutputCache(Duration = 1, VaryByParam = "*")]
        public PartialViewResult Services()
        {
            ViewBag.EmployersNumber = db.Employers.Count();
            ViewBag.EmployeesNumber = db.Employees.Count();
            ViewBag.JobsNumber = db.Jobs.Count();

            return PartialView("_Services");
        }

        public ActionResult Register()
        {
            //if account is already login, redirect to home page 
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", SecurityUtilities.GetAuthenticatedUserType());
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "FName,LName,Email,PW")]User user)
        {
            if (ModelState.IsValid)
            {
                var isExist = EmailUtilities.IsEmailExists(user.Email);
                if (isExist)
                {
                    //email is registered before
                    ModelState.AddModelError("EmailExist", "This email already exsist. ");
                    return View(user);
                }

                //Completing user model data
                user.PW = SecurityUtilities.Hash(user.PW);
                user.ID = Guid.NewGuid();
                user.Code = Guid.NewGuid();

                //saving to db
                using (JobbyEntities db = new JobbyEntities())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // insert user data in user and employee tables in one transaction
                            db.Users.Add(user);
                            db.SaveChanges();
                            db.Employees.Add(new Employee() { UserID = user.ID });
                            db.SaveChanges();
                            Guid roleId = db.Roles.Where(r => r.Name == "Employee").Select(r => r.ID).FirstOrDefault();
                            db.UserRoles.Add(new UserRole() { UserID = user.ID, RoleID = roleId });
                            db.SaveChanges();

                            // send verification email
                            EmailUtilities.SendActivationLinkEmail(user.Email, user.Code.ToString());

                            //Registeration succeeded, Sign in this account 
                            Response.Cookies.Add(SecurityUtilities.CreateAuthenticationCookie(user.FName, user.ID, "Employee"));

                            // commit now
                            transaction.Commit();
                            // Redirect to Account Activation page
                            return RedirectToAction("VerifyAccount", "User");
                        }
                        catch (Exception ex)
                        {
                            // roll back all database operations, if any thing goes wrong
                            ModelState.AddModelError("RegisterError", "An error occured While registeration process. ");
                            transaction.Rollback();
                            return View(user);
                        }
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return View(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}