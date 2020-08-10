using Jobby.Models;
using Jobby.Models.Extended;
using Jobby.Utilities;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Jobby.Controllers
{
    public class UserController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
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
        public ActionResult Login(LoginViewModel LoginVM)
        {
            if (ModelState.IsValid)
            {
                using (JobbyEntities db = new JobbyEntities())
                {
                    var user = db.Users.Where(u => u.Email == LoginVM.Email).FirstOrDefault();
                    if (user != null)
                    {
                        //valid email address
                        if (user.LockoutEndDate == null || user.LockoutEndDate < DateTime.Now)
                        {
                            //user not blocked
                            if (string.Compare(SecurityUtilities.Hash(LoginVM.PW), user.PW) == 0)
                            {
                                //valid login password, reset access faild counter and create FormAuthentication Cookie
                                user.AccessFaildCount = 0;
                                db.SaveChanges();
                                string userType = user.UserRoles.Where(u => u.UserID == user.ID).Select(u => u.Role.Name).FirstOrDefault().ToString();
                                Response.Cookies.Add(SecurityUtilities.CreateAuthenticationCookie(user.FName, user.ID, userType));
                                
                                if (user.IsVerified)
                                {
                                    //account is active, redirect to home page
                                    return RedirectToAction("Index", userType);
                                }
                                else
                                {
                                    //account needs activation, redirect to account activation page
                                    return RedirectToAction("VerifyAccount");
                                }
                            }
                            else
                            {
                                //invalid password
                                if (user.AccessFaildCount >= 2)
                                {
                                    //invalid password for 3 times, block this account for 60 minutes.
                                    user.LockoutEndDate = DateTime.Now.AddMinutes(60);
                                    user.AccessFaildCount = 0;
                                    db.SaveChanges();
                                    var LockEndDate = user.LockoutEndDate.Value.ToString("HH:mm tt");
                                    ModelState.AddModelError("BlockedUser", "Account is blocked untill : " + LockEndDate + ". ");
                                }
                                else
                                {
                                    //invalid password, increase access faild counter
                                    user.AccessFaildCount += 1;
                                    db.SaveChanges();
                                    ModelState.AddModelError("InvalidPassword", "Invalid Password. ");
                                }
                            }
                        }
                        else
                        {
                            //account is blocked
                            var LockEndDate = user.LockoutEndDate.Value.ToString("HH:mm tt");
                            ModelState.AddModelError("BlockedUser", "Account is blocked untill : " + LockEndDate + ". ");
                        }
                    }
                    else
                    {
                        //wrong email address
                        ModelState.AddModelError("InvalidEmail", "Invalid Email Address. ");
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return View(LoginVM);
        }

        [Authorize]
        public ActionResult VerifyAccount()
        {
            string userType = SecurityUtilities.GetAuthenticatedUserType();
            //redirect to home page if account is already active
            using (JobbyEntities db = new JobbyEntities())
            {
                Guid id = SecurityUtilities.GetAuthenticatedUserID();
                var user = db.Users.Where(u => u.ID == id).FirstOrDefault();
                if (user!=null && user.IsVerified == true)
                {
                    return RedirectToAction("Index", userType);
                }
            }
            ViewBag.UserType = userType;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyAccount(string code)
        {
            string userType = SecurityUtilities.GetAuthenticatedUserType();
            var GCode = new Guid();
            bool isValid = Guid.TryParse(code, out GCode);
            if (isValid)
            {
                //valid guid code, check if exists
                using (JobbyEntities db = new JobbyEntities())
                {
                    var user = db.Users.Where(u => u.Code == GCode).FirstOrDefault();
                    if (user != null)
                    {
                        //user with this code exists, check if this account is not activated before.
                        if (!user.IsVerified)
                        {
                            //activate this account
                            user.IsVerified = true;
                            user.Code = Guid.NewGuid();
                            db.SaveChanges();
                            return RedirectToAction("Index", userType);
                        }
                        else
                        {
                            //account is already activated
                            ModelState.AddModelError("AlreadyActivated", "Account already activated. ");
                        }
                    }
                    else
                    {
                        //there is no user with this code
                        ModelState.AddModelError("invalidCode", "Invalid activation code. ");
                    }
                }
            }
            else 
            {
                //invalid guid code format
                ModelState.AddModelError("invalidCode", "Invalid activation code. ");
            }
            // If we got this far, something failed, redisplay form
            ViewBag.UserType = userType ;
            return View();
        }

        [Authorize]
        public ActionResult ResendActivationCode() 
        {
            using (JobbyEntities db = new JobbyEntities())
            {
                Guid id = SecurityUtilities.GetAuthenticatedUserID();
                var user = db.Users.Where(u => u.ID == id).FirstOrDefault();
                if (user != null)
                {
                    //check if user account not already verified
                    if (!user.IsVerified)
                    {
                        //generate new activation code then send it to user email
                        user.Code = Guid.NewGuid();
                        db.SaveChanges();
                        EmailUtilities.SendActivationLinkEmail(user.Email, user.Code.ToString(), "ResendCode");
                        ViewBag.State = true;
                        ViewBag.Message = "Activation code sent successfully.";
                        return PartialView("_ResendActivationCode");
                    }
                    else 
                    {
                        //account is active
                        ViewBag.State = false;
                        ViewBag.Message = "Account already activated.";
                        return PartialView("_ResendActivationCode");
                    }
                }
            }
            // If we got this far, somthing went wrong
            return RedirectToAction("VerifyAccount");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
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
        public ActionResult ForgotPassword(string userEmail)
        {
            using (JobbyEntities db = new JobbyEntities())
            {
                var user = db.Users.Where(u => u.Email == userEmail).FirstOrDefault();
                if (user != null)
                {
                    //email is valid , generate activation code and send it to email
                    user.Code = Guid.NewGuid();
                    db.SaveChanges();
                    EmailUtilities.SendActivationLinkEmail(userEmail, user.Code.ToString(), "ResetPassword");
                    TempData["CodeSent"] = true;
                    return RedirectToAction("ResetPassword");
                }
                else
                {
                    //wrong email address
                    ModelState.AddModelError("InvalidEmail", "Invalid Email Address. ");
                }
            }
            // If we got this far, something failed, redisplay form
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword() 
        {
            //if account is already login, redirect to home page 
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", SecurityUtilities.GetAuthenticatedUserType());
            }
            //redirect to forgot password if forgot-password-code didn't sent to email
            if (TempData.Peek("CodeSent") == null)
            {
                return RedirectToAction("ForgotPassword");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel ResetPasswordVM)
        {
            if (ModelState.IsValid) 
            {
                using (JobbyEntities db = new JobbyEntities())
                {
                    var GCode = new Guid();
                    bool isValid = Guid.TryParse(ResetPasswordVM.Code, out GCode);
                    if (isValid)
                    {
                        //valid Guid code
                        var User = db.Users.Where(user => user.Code == GCode).FirstOrDefault();
                        if (User != null)
                        {
                            //code is correct, accept changing password
                            User.PW = SecurityUtilities.Hash(ResetPasswordVM.Password);
                            User.Code = Guid.NewGuid();
                            db.SaveChanges();
                            //clear temp data to prevent direct access to reset password
                            TempData.Remove("CodeSent");
                            return RedirectToAction("Login");
                        }
                    }
                    //invalid activation code
                    ModelState.AddModelError("InvalidCode", "Invalid code. ");
                }
            }
            // If we got this far, something failed, redisplay form
            return View(ResetPasswordVM);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.UserType = SecurityUtilities.GetAuthenticatedUserType();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel ChangePasswordVM)
        {
            string userType = SecurityUtilities.GetAuthenticatedUserType();
            if (ModelState.IsValid)
            {
                using (JobbyEntities db = new JobbyEntities())
                {
                    //find user
                    Guid id = SecurityUtilities.GetAuthenticatedUserID();
                    var User = db.Users.Where(user => user.ID == id).FirstOrDefault();
                    // comparinf hashed password
                    if (string.Compare(User.PW, SecurityUtilities.Hash(ChangePasswordVM.CurrentPassword)) == 0)
                    {
                        //password is correct, accept changing password
                        User.PW = SecurityUtilities.Hash(ChangePasswordVM.NewPassword);
                        db.SaveChanges();
                        return RedirectToAction("Index", userType);
                    }
                //invalid password
                ModelState.AddModelError("InvalidPassword", "Invalid Password. ");
                }
            }
            // If we got this far, something failed, redisplay form
            ViewBag.UserType = userType;
            return View(ChangePasswordVM);
        }

        [Authorize]
        public ActionResult ChangeEmail()
        {
            ViewBag.UserType = SecurityUtilities.GetAuthenticatedUserType();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeEmail(string userEmail)
        {
            using (JobbyEntities db = new JobbyEntities())
            {
                Guid id = SecurityUtilities.GetAuthenticatedUserID();
                var user = db.Users.Where(u => u.ID == id).FirstOrDefault();
                if (string.Compare(user.Email, userEmail) == 0)
                {
                    ModelState.AddModelError("InvalidEmail", "New Email matches current one. ");
                    ViewBag.UserType = SecurityUtilities.GetAuthenticatedUserType();
                    return View();
                }
                user.Code = Guid.NewGuid();
                user.Email = userEmail;
                user.IsVerified = false;
                db.SaveChanges();
                EmailUtilities.SendActivationLinkEmail(userEmail, user.Code.ToString(), "ResendCode");
            }
            return RedirectToAction("VerifyAccount");
        }

        [HttpPost]
        public ActionResult ChangeImg(HttpPostedFileBase upload)
        {
            // get user from database
            Guid id = SecurityUtilities.GetAuthenticatedUserID();
            string controller = SecurityUtilities.GetAuthenticatedUserType();
            using (JobbyEntities db = new JobbyEntities())
            {
                var user = db.Users.Where(u => u.ID == id).FirstOrDefault();
                if (upload != null && upload.ContentLength > 0)
                {
                    // convert image to array of binary then store it into database
                    user.Img = new byte[upload.ContentLength];
                    upload.InputStream.Read(user.Img, 0, upload.ContentLength);
                    db.SaveChanges();
                }
                if (string.Compare(controller, "Employee") == 0)
                {
                    //redirect to profile after changing profile picture  
                    return RedirectToAction("Index", "EmployeeProfile");
                }
                //redirect to profile after changing profile picture  
                return RedirectToAction("Profile", controller);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Logout() {
            string userType = SecurityUtilities.GetAuthenticatedUserType(); 
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", userType);
        }
    }
}