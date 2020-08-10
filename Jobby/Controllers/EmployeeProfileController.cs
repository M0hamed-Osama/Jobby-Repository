using Jobby.Models;
using Jobby.Models.Extended;
using Jobby.Utilities;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Jobby.Controllers
{
    [Authorize]
    public class EmployeeProfileController : Controller
    {
        private JobbyEntities db = new JobbyEntities();

        public ActionResult Index()
        {
            string controller = SecurityUtilities.GetAuthenticatedUserType();
            if (controller != "Employee")
            {
                //redirect to home if request isn't from employee
                return RedirectToAction("Index", controller);
            }
            Guid id = SecurityUtilities.GetAuthenticatedUserID();
            var user = db.Users.Where(u => u.ID == id).FirstOrDefault();
            return View(user);
        }

        public ActionResult PersonalInformation() 
        {
            Guid id = SecurityUtilities.GetAuthenticatedUserID();
            var user = db.Users.Where(u => u.ID == id).FirstOrDefault();
            var employee = db.Employees.Where(emp => emp.UserID == id).FirstOrDefault();
            PersonalInfoViewModel PersonalInfoVM = new PersonalInfoViewModel() {
            FName=user.FName,
            LName=user.LName,
            CountryID=employee.CountryID,
            City=employee.City,
            PhoneNumber = employee.PhoneNumber,
            BirthDate=employee.BirthDate,
            MartialStatus=employee.MartialStatus,
            MilitaryStatus=employee.MilitaryStatus
            };
            ViewBag.CountryID = new SelectList(db.Countries, "Id", "Name", employee.CountryID);
            return View(PersonalInfoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PersonalInformation(PersonalInfoViewModel PersonalInformationVM)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Guid id = SecurityUtilities.GetAuthenticatedUserID();
                        var user = db.Users.Where(u => u.ID == id).FirstOrDefault();
                        user.FName = PersonalInformationVM.FName;
                        user.LName = PersonalInformationVM.LName;
                        db.SaveChanges();

                        var employee = db.Employees.Where(emp => emp.UserID == id).FirstOrDefault();
                        employee.CountryID = PersonalInformationVM.CountryID;
                        employee.City = PersonalInformationVM.City;
                        employee.PhoneNumber = PersonalInformationVM.PhoneNumber;
                        employee.BirthDate = PersonalInformationVM.BirthDate;
                        employee.MartialStatus = PersonalInformationVM.MartialStatus;
                        employee.MilitaryStatus = PersonalInformationVM.MilitaryStatus;
                        db.SaveChanges();

                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception) 
                    {
                        ModelState.AddModelError("SaveFaild", "Save changes faild.");
                        transaction.Rollback();
                        ViewBag.CountryID = new SelectList(db.Countries, "Id", "Name", PersonalInformationVM.CountryID);
                        return View(PersonalInformationVM);
                    }
                }
            }
            // if we got here , an error occured
            ViewBag.CountryID = new SelectList(db.Countries, "Id", "Name", PersonalInformationVM.CountryID);
            return View(PersonalInformationVM);
        }

        public ActionResult EducationInformation()
        {
            Guid id = SecurityUtilities.GetAuthenticatedUserID();
            var userEducation = db.UserEducations.Where(u => u.UserID == id).FirstOrDefault();
            return View(userEducation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EducationInformation([Bind(Include ="Degree,Entity,Field,Grade,StartYear,EndYear")]UserEducation userEducation)
        {
            if (ModelState.IsValid)
            {
                Guid id = SecurityUtilities.GetAuthenticatedUserID();
                userEducation.UserID = id;
                var result = db.UserEducations.Where(u => u.UserID == id).FirstOrDefault();
                if (result == null)
                {
                    db.UserEducations.Add(userEducation);
                    db.SaveChanges();
                }
                else 
                {
                    result.UserID = userEducation.UserID;
                    result.StartYear = userEducation.StartYear;
                    result.EndYear = userEducation.EndYear;
                    result.Degree = userEducation.Degree;
                    result.Field = userEducation.Field;
                    result.Entity = userEducation.Entity;
                    result.Grade = userEducation.Grade;
                    db.Entry(result).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            // if we got here , an error occured
            return View(userEducation);
        }

        public ActionResult WorkInformation()
        {
            Guid id = SecurityUtilities.GetAuthenticatedUserID();
            var employee = db.Employees.Where(emp => emp.UserID == id).FirstOrDefault();
            ViewBag.CvExist = (employee.CV != null) ? true : false;
            WorkInfoViewModel WorkInfoVM = new WorkInfoViewModel() {
                Salary = employee.Salary,
                JobStatus = employee.JobStatus,
                CareerLevel = employee.CareerLevel,
            };
            return View(WorkInfoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WorkInformation(WorkInfoViewModel WorkInfoVM)
        {
            if (ModelState.IsValid)
            {
                Guid id = SecurityUtilities.GetAuthenticatedUserID();
                var employee = db.Employees.Where(emp => emp.UserID == id).FirstOrDefault();
                employee.Salary = WorkInfoVM.Salary;
                employee.JobStatus = WorkInfoVM.JobStatus;
                employee.CareerLevel = WorkInfoVM.CareerLevel;
                if (WorkInfoVM.CV != null && WorkInfoVM.CV.ContentLength > 0)
                {
                    // convert cv to array of binary then store it into database
                    Stream stream = WorkInfoVM.CV.InputStream;
                    BinaryReader Br = new BinaryReader(stream);
                    employee.CV = Br.ReadBytes((int)stream.Length);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            // if we got here , an error occured
            return View(WorkInfoVM);
        }
        
        public FileResult DownloadCV(Guid? id)
        {
            if (id==null)
            {
                id = SecurityUtilities.GetAuthenticatedUserID();
            }
            var employee = db.Employees.Where(emp => emp.UserID == id).FirstOrDefault();
            return File(employee.CV, "application/pdf", User.Identity.Name + " Cv.pdf");
        }

        public ActionResult SkillsInformation() 
        {
            Guid id = SecurityUtilities.GetAuthenticatedUserID();
            var skills = db.UserSkills.Where(s => s.UserID == id).OrderByDescending(s=>s.SkillLevel);
            if (skills != null) 
            {
                return View(skills.ToList());
            }
            return View();
        }

        public ActionResult AddUserSkill()
        {
            return View("UserSkill");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUserSkill([Bind(Include ="SkillName,SkillLevel,IsLanguage")]UserSkill userSkill)
        {
            if (ModelState.IsValid)
            {
                Guid id = SecurityUtilities.GetAuthenticatedUserID();
                userSkill.UserID = id;
                userSkill.ID = Guid.NewGuid();
                db.UserSkills.Add(userSkill);
                db.SaveChanges();
                return RedirectToAction("SkillsInformation");
            }
            // if we got here , an error occured
            return View("UserSkill",userSkill);
        }

        public ActionResult EditUserSkill(Guid ID)
        {
            var result = db.UserSkills.Where(s => s.ID == ID).FirstOrDefault();
            if (result != null)
            {
                return View("UserSkill",result);
            }
            return RedirectToAction("SkillsInformation");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserSkill([Bind(Include = "ID,UserID,SkillName,SkillLevel,IsLanguage")]UserSkill userSkill)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userSkill).State=EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SkillsInformation");
            }
            // if we got here , an error occured
            return View(userSkill);
        }
        
        [HttpPost]
        public PartialViewResult DeleteUserSkill(Guid ID)
        {
            var result = db.UserSkills.Where(s => s.ID == ID).FirstOrDefault();
            db.UserSkills.Remove(result);
            db.SaveChanges();
            var skills = db.UserSkills.Where(s => s.UserID == result.UserID).OrderByDescending(s => s.SkillLevel);
            if (skills != null)
            {
                return PartialView("_SkillsInformation", skills.ToList());
            }
            return PartialView("_SkillsInformation");
        }

        public new ActionResult Profile(Guid ID)
        {
            string controller = SecurityUtilities.GetAuthenticatedUserType();
            if (controller != "Employer")
            {
                //redirect to home if request isn't from employee
                return RedirectToAction("Index", controller);
            }
            var user = db.Users.Where(u => u.ID == ID).FirstOrDefault();
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