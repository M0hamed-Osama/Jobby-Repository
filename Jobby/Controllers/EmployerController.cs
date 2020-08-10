using Jobby.Models;
using Jobby.Models.Extended;
using Jobby.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Jobby.Controllers
{
    public class EmployerController : Controller
    {
        private JobbyEntities db = new JobbyEntities();

        // GET: Employer/Index
        [AllowAnonymous]
        public ActionResult Index()
        {
            //if account is already login, redirect to home page 
            if (Request.IsAuthenticated)
            {
                if (SecurityUtilities.GetAuthenticatedUserType() == "Employee")
                    return RedirectToAction("Index", "Employee");
            }
            return View();
        }

        [HttpGet]
        [OutputCache(Duration = 1, VaryByParam = "*")]
        public PartialViewResult TopCompanies()
        {
            var employers_ids = db.Jobs.GroupBy(j => j.UserID).OrderByDescending(j => j.Count()).Select(j => j.Key).Take(3).ToList();

            List<Employer> employers = new List<Employer>();
            foreach (var id in employers_ids)
            {
                employers.Add(db.Employers.Where(emp => emp.UserID == id).FirstOrDefault());
            }
            return PartialView("_TopCompanies", employers);
        }

        // GET: /Employer/Register
        [AllowAnonymous]
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
        public ActionResult Register(EmployerRegisterViewModel EmployerRegisterVM)
        {
            if (ModelState.IsValid)
            {
                var isExist = EmailUtilities.IsEmailExists(EmployerRegisterVM.Email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "This email already exsist. ");
                    return View(EmployerRegisterVM);
                }
                var user = new User()
                {
                    FName = EmployerRegisterVM.FName,
                    LName = EmployerRegisterVM.LName,
                    Email = EmployerRegisterVM.Email,
                    PW = SecurityUtilities.Hash(EmployerRegisterVM.PW),
                    ID = Guid.NewGuid(),
                    Code = Guid.NewGuid(),
                };
                using (JobbyEntities db = new JobbyEntities())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.Users.Add(user);
                            db.SaveChanges();
                            db.Employers.Add(new Employer()
                            {
                                UserID = user.ID,
                                CompanyName = EmployerRegisterVM.CName,
                                Indusrty = EmployerRegisterVM.Industry,
                                Website = EmployerRegisterVM.Website
                            });
                            db.SaveChanges();
                            Guid roleId = db.Roles.Where(r => r.Name == "Employer").Select(r => r.ID).FirstOrDefault();
                            db.UserRoles.Add(new UserRole() { UserID = user.ID, RoleID = roleId });
                            db.SaveChanges();
                            transaction.Commit();
                            EmailUtilities.SendActivationLinkEmail(user.Email, user.Code.ToString());

                            //Registeration succeeded, Sign in this account
                            Response.Cookies.Add(SecurityUtilities.CreateAuthenticationCookie(user.FName, user.ID, "Employer"));

                            // Redirect to Account Activation page
                            return RedirectToAction("VerifyAccount", "User");
                        }
                        catch (Exception)
                        {
                            // roll back all database operations, if any thing goes wrong
                            ModelState.AddModelError("RegisterError", "An error occured While registeration process. ");
                            transaction.Rollback();
                            return View(EmployerRegisterVM);
                        }
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return View(EmployerRegisterVM);
        }

        //GET: /Employer/Profile
        [Authorize]
        public new ActionResult Profile()
        {
            string controller = SecurityUtilities.GetAuthenticatedUserType();
            if (controller != "Employer")
            {
                //redirect to home if user isn't an employer
                return RedirectToAction("Index", controller);
            }
            Guid id = SecurityUtilities.GetAuthenticatedUserID();
            using (JobbyEntities db = new JobbyEntities()) 
            {
                var user = db.Users.Where(u => u.ID == id).FirstOrDefault();
                var employer = db.Employers.Where(emp => emp.UserID == id).FirstOrDefault();
                EmployerProfileViewModel VM = new EmployerProfileViewModel()
                {
                    FName = user.FName,
                    LName = user.LName,
                    Img = user.Img,
                    CName = employer.CompanyName,
                    Industry = employer.Indusrty,
                    Website = employer.Website
                };
                return View(VM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Profile([Bind(Exclude ="Img")]EmployerProfileViewModel VM)
        {
            if (ModelState.IsValid)
            {
                using (JobbyEntities db = new JobbyEntities()) 
                {

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            Guid id = SecurityUtilities.GetAuthenticatedUserID();
                            var user = db.Users.Where(u => u.ID == id).FirstOrDefault();
                            user.FName = VM.FName;
                            user.LName = VM.LName;
                            db.SaveChanges();

                            var employer = db.Employers.Where(emp => emp.UserID == id).FirstOrDefault();
                            employer.CompanyName = VM.CName;
                            employer.Indusrty = VM.Industry;
                            employer.Website = VM.Website;
                            db.SaveChanges();

                            transaction.Commit();
                            return RedirectToAction("Index");
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("SaveFaild", "Save changes faild.");
                            transaction.Rollback();
                            return View(VM);
                        }
                    }
                }
            }
            // if we got here , an error occured
            return View(VM);
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