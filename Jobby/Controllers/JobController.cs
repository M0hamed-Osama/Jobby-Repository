using Jobby.Models;
using Jobby.Models.Extended;
using Jobby.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Jobby.Controllers
{
    public class JobController : Controller
    {
        private JobbyEntities db = new JobbyEntities();
        
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                string controller = SecurityUtilities.GetAuthenticatedUserType();
                ViewBag.UserType = controller;
                if (controller == "Employee")
                {
                    Guid id = SecurityUtilities.GetAuthenticatedUserID();
                    ViewBag.savedJobs = db.SavedJobs.Where(j => j.UserID == id).Select(j => j.JobID).ToList();
                }
            }
            return View(db.Jobs.OrderByDescending(j => j.PostDate).ToList());
        }

        [HttpPost]
        public ActionResult Index(string search)
        {
            if (Request.IsAuthenticated)
            {
                string controller = SecurityUtilities.GetAuthenticatedUserType();
                ViewBag.UserType = controller;
                if (controller == "Employee")
                {
                    Guid id = SecurityUtilities.GetAuthenticatedUserID();
                    ViewBag.savedJobs = db.SavedJobs.Where(j => j.UserID == id).Select(j => j.JobID).ToList();
                }
            }
            return View("Index",db.Jobs.Where(j => j.Title.ToLower().Contains(search.ToLower()) || j.JobDesc.ToLower().Contains(search.ToLower())).OrderByDescending(j => j.PostDate).ToList());
        }

        public ActionResult Job(Guid id)
        {
            if (Request.IsAuthenticated)
            {
                string controller = SecurityUtilities.GetAuthenticatedUserType();
                ViewBag.UserType = controller;
                if (controller == "Employee")
                {
                    Guid userID = SecurityUtilities.GetAuthenticatedUserID();
                    if (db.SavedJobs.Where(j => j.UserID == userID && j.JobID == id).FirstOrDefault()!=null)
                    {
                        ViewBag.isSaved = true;
                    }
                    if (db.Applications.Where(a => a.UserID == userID && a.JobID == id).FirstOrDefault() != null)
                    {
                        ViewBag.isApplied = true;
                    }
                }              
            }
            return View(db.Jobs.Where(j => j.ID==id).FirstOrDefault());
        }

        [HttpPost]
        public PartialViewResult Search(string search)
        {
            List<Job> FoundedJobs;
            if (!string.IsNullOrWhiteSpace(search))
            {
                FoundedJobs = db.Jobs.Where(j => j.Title.ToLower().Contains(search.ToLower()) || j.JobDesc.ToLower().Contains(search.ToLower())).OrderByDescending(j => j.PostDate).ToList();
            }
            else 
            {
                FoundedJobs = db.Jobs.OrderByDescending(j => j.PostDate).ToList();
            }
            //mark saved jobs 
            if (Request.IsAuthenticated)
            {
                string controller = SecurityUtilities.GetAuthenticatedUserType();
                if (controller == "Employee" && FoundedJobs.Count > 0)
                {
                    Guid id = SecurityUtilities.GetAuthenticatedUserID();
                    ViewBag.savedJobs = db.SavedJobs.Where(j => j.UserID == id).Select(j => j.JobID).ToList();
                }
            }
            return PartialView("_Jobs", FoundedJobs);
        }

        [HttpPost]
        public PartialViewResult Filter(FilterViewModel FilterVM)
        {
            List<Job> jobs = new List<Job>();
            bool careerFilters , dateFilters , countryFilters, salaryFilters;
            careerFilters = dateFilters = countryFilters = salaryFilters = false;

            // career level filters
            if (FilterVM.CareerLevelFilters.Any(f => f.IsChecked == true))
            {
                careerFilters = true;
                foreach (var filter in FilterVM.CareerLevelFilters)
                {
                    if (filter.IsChecked)
                    {
                        jobs = jobs.Union(db.Jobs.Where(j => j.CareerLevel == filter.Value)).ToList();
                    }
                }
            }

            // post date filters
            if (FilterVM.PostDateFilters.Any(f => f.IsChecked == true))
            {
                //only apply this filter if other filters have results or they are not appplied
                if (jobs.Count > 0 || ! careerFilters )
                {
                    dateFilters = true;
                    for (int i = FilterVM.PostDateFilters.Count - 1; i >= 0; i--)
                    {
                        if (FilterVM.PostDateFilters[i].IsChecked)
                        {
                            var date = DateTime.Now.AddDays(0 - Convert.ToInt32(FilterVM.PostDateFilters[i].Value));
                            if (careerFilters)
                            {
                                jobs = jobs.Intersect(jobs.Where(j => j.PostDate > date)).ToList();
                            }
                            else 
                            {
                                jobs = db.Jobs.Where(j => j.PostDate > date).ToList();
                            }
                            break;
                        }
                    }
                }
            }

            // country filters
            if (FilterVM.CountryFilters.Any(f => f.IsChecked == true))
            {
                //only apply this filter if other filters have results or they are not appplied
                if (jobs.Count > 0  || !(careerFilters || dateFilters))
                {
                    List<Job> matchedJobs = new List<Job>();
                    countryFilters = true;
                    foreach (var filter in FilterVM.CountryFilters)
                    {
                        if (filter.IsChecked)
                        {
                            if (careerFilters || dateFilters)
                            {
                                matchedJobs = matchedJobs.Union(jobs.Where(j => j.Country.Name == filter.Value)).ToList();

                            }
                            else
                            {
                                matchedJobs = matchedJobs.Union(db.Jobs.Where(j => j.Country.Name == filter.Value)).ToList();

                            }
                        }
                    }
                    if (careerFilters || dateFilters)
                    {
                        jobs = jobs.Intersect(matchedJobs).ToList();
                    }
                    else
                    {
                        jobs = matchedJobs;
                    }
                }
            }

            // salary filters
            if (FilterVM.SalaryFilters.Any(f => f.IsChecked == true))
            {
                //only apply this filter if other filters have results or they are not appplied
                if (jobs.Count > 0 || !(careerFilters || dateFilters || countryFilters))
                {
                    salaryFilters = true;
                    for (int i = 0 ; i < FilterVM.SalaryFilters.Count ; i++)
                    {
                        if (FilterVM.SalaryFilters[i].IsChecked)
                        {
                            int salary = Convert.ToInt32(FilterVM.SalaryFilters[i].Value);
                            if (careerFilters || dateFilters || countryFilters)
                            {
                                jobs = jobs.Intersect(db.Jobs.Where(j => j.Salary > salary)).ToList();
                            }
                            else
                            {
                                jobs = db.Jobs.Where(j => j.Salary > salary).ToList();
                            }
                            break;
                        }
                    }
                } 
            }
            
            // retrieve all jobs if no filter is selected
            if (!(careerFilters || dateFilters || countryFilters || salaryFilters))
            {
                jobs = db.Jobs.ToList();
            }

            //mark saved jobs 
            if (Request.IsAuthenticated)
            {
                string controller = SecurityUtilities.GetAuthenticatedUserType();
                if (controller == "Employee" && jobs.Count > 0)
                {
                    Guid id = SecurityUtilities.GetAuthenticatedUserID();
                    ViewBag.savedJobs = db.SavedJobs.Where(j => j.UserID == id).Select(j => j.JobID).ToList();
                }
            }
          
            return PartialView("_Jobs", jobs.OrderByDescending(j => j.PostDate).ToList());
        }

        [Authorize]
        public ActionResult PostedJobs()
        {
            string controller = SecurityUtilities.GetAuthenticatedUserType();
            if (controller != "Employer")
            {
                //redirect to home if user isn't an employer
                return RedirectToAction("Index", controller);
            }
            Guid id = SecurityUtilities.GetAuthenticatedUserID();
            var jobs = db.Jobs.Where(j=>j.UserID==id).OrderByDescending(j => j.PostDate).ToList();
            return View(jobs);
        }

        [Authorize]
        public ActionResult Create()
        {
            string controller = SecurityUtilities.GetAuthenticatedUserType();
            if (controller != "Employer")
            {
                //redirect to home if user isn't an employer
                return RedirectToAction("Index", controller);
            }
            //filling model data from database
            Guid id = SecurityUtilities.GetAuthenticatedUserID();
            var employer = db.Employers.Where(emp => emp.UserID == id).FirstOrDefault();
            var user = db.Users.Where(u => u.ID == id).FirstOrDefault();
            ViewBag.Company = employer.CompanyName;
            ViewBag.CountryID = new SelectList(db.Countries, "Id", "Name");
            ViewBag.JobFieldID = new SelectList(db.JobFields, "Id", "Name");
            ViewBag.Img = user.Img;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,JobFieldID,CareerLevel,Salary,CountryID,City,JobDesc")]Job job)
        {
            Guid id = SecurityUtilities.GetAuthenticatedUserID();
            if (ModelState.IsValid)
            {
                job.ID = Guid.NewGuid();
                job.UserID = id;
                job.PostDate = DateTime.Now;
                job.JobStatus = "o";
                db.Jobs.Add(job);
                db.SaveChanges();
                return RedirectToAction("PostedJobs");
            }
            ModelState.AddModelError("Error", "An error occured. ");
            //filling model data from database
            var employer = db.Employers.Where(emp => emp.UserID == id).FirstOrDefault();
            var user = db.Users.Where(u => u.ID == id).FirstOrDefault();
            ViewBag.Company = employer.CompanyName;
            ViewBag.CountryID = new SelectList(db.Countries, "Id", "Name");
            ViewBag.JobFieldID = new SelectList(db.JobFields, "ID", "Name");
            ViewBag.Img = user.Img;
            return View(job);
        }

        [HttpPost]
        public PartialViewResult Delete(Guid id)
        {
            var job = db.Jobs.Where(j => j.ID == id).FirstOrDefault();
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //remove job from saved jobs table first
                    var allSavedJobs = db.SavedJobs.Where(j => j.JobID == id).ToList();
                    foreach (var saved in allSavedJobs)
                    {
                        db.SavedJobs.Remove(saved);
                    }
                    var allApplications = db.Applications.Where(a => a.JobID == id).ToList();
                    foreach (var application in allApplications)
                    {
                        db.Applications.Remove(application);
                    }
                    db.Jobs.Remove(job);
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    // roll back all database operations, if any thing goes wrong
                    transaction.Rollback();
                }

            }
            var jobs = db.Jobs.Where(j => j.UserID == job.UserID);
            return PartialView("_PostedJobs", jobs.ToList());
        }

        [Authorize]
        public ActionResult Edit(Guid id)
        {
            string controller = SecurityUtilities.GetAuthenticatedUserType();
            Guid userID = SecurityUtilities.GetAuthenticatedUserID();
            if (controller == "Employer")
            {
                var job = db.Jobs.Where(j => j.ID == id).FirstOrDefault();
                if (job.UserID == userID)
                {
                    //filling model data from database
                    ViewBag.CountryID = new SelectList(db.Countries, "Id", "Name", job.CountryID);
                    ViewBag.JobFieldID = new SelectList(db.JobFields, "ID", "Name", job.JobFieldID);
                    return View(job);
                }
            }
            //redirect to home if user isn't an employer
            return RedirectToAction("Index", controller);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,JobStatus,PostDate,Title,JobFieldID,CareerLevel,Salary,CountryID,City,JobDesc")]Job job)
        {
            if (ModelState.IsValid)
            {
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("PostedJobs");
            }
            ModelState.AddModelError("Error", "An error occured. ");
            //filling model data from database
            ViewBag.CountryID = new SelectList(db.Countries, "Id", "Name", job.CountryID);
            ViewBag.JobFieldID = new SelectList(db.JobFields, "ID", "Name", job.JobFieldID);
            return View(job);
        }

        [Authorize]
        public ActionResult SavedJobs()
        {
            string controller = SecurityUtilities.GetAuthenticatedUserType();
            if (controller != "Employee")
            {
                //redirect to home if user isn't an employee
                return RedirectToAction("Index", controller);
            }
            Guid userID = SecurityUtilities.GetAuthenticatedUserID();
            var jobs = db.SavedJobs.Where(j => j.UserID == userID).OrderByDescending(j => j.Job.PostDate).ToList();
            ViewBag.appliedJobs = new List<Guid>();
            if (jobs.Count > 0)
            {
                ViewBag.appliedJobs = db.Applications.Where(j => j.UserID == userID).Select(j => j.JobID).ToList();
            }
            return View(jobs);
        }

        public PartialViewResult SaveJob(Guid id)
        {
            Guid userID = SecurityUtilities.GetAuthenticatedUserID();
            db.SavedJobs.Add(new SavedJob() { JobID = id, UserID = userID });
            db.SaveChanges();
            ViewBag.isSaved = true;
            return PartialView("_SaveIcon",id);
        }
        
        public PartialViewResult UnSaveJob(Guid id)
        {
            Guid userID = SecurityUtilities.GetAuthenticatedUserID();
            var job = db.SavedJobs.Where(j => j.UserID == userID && j.JobID == id).FirstOrDefault();
            db.SavedJobs.Remove(job);
            db.SaveChanges();
            ViewBag.isSaved = false;
            return PartialView("_SaveIcon", id);
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult DeleteSavedJob(Guid id)
        {
            Guid userID = SecurityUtilities.GetAuthenticatedUserID();
            var job = db.SavedJobs.Where(j => j.UserID == userID && j.JobID == id).FirstOrDefault();
            db.SavedJobs.Remove(job);
            db.SaveChanges();
            var jobs = db.SavedJobs.Where(j => j.UserID == userID).OrderByDescending(j => j.Job.PostDate).ToList();
            ViewBag.appliedJobs = new List<Guid>();
            if (jobs.Count > 0)
            {
                ViewBag.appliedJobs = db.Applications.Where(j => j.UserID == userID).Select(j => j.JobID).ToList();
            }
            return PartialView("_SavedJobs", jobs.ToList());
        }
        
        [Authorize]
        public ActionResult Applications()
        {
            string controller = SecurityUtilities.GetAuthenticatedUserType();
            if (controller != "Employee")
            {
                //redirect to home if user isn't an employee
                return RedirectToAction("Index", controller);
            }
            Guid userID = SecurityUtilities.GetAuthenticatedUserID();
            var applications = db.Applications.Where(a => a.UserID == userID).OrderByDescending(a => a.Job.PostDate).ToList();
            return View(applications);
        }

        [Authorize]
        public ActionResult ApplyForJob(Guid id)
        {
            Guid userID = SecurityUtilities.GetAuthenticatedUserID();
            db.Applications.Add(new Application() { JobID = id, UserID = userID , ApplicationStatus="Pending"});
            db.SaveChanges();
            return RedirectToAction("Applications");
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult DeleteApplication(Guid id)
        {
            Guid userID = SecurityUtilities.GetAuthenticatedUserID();
            var job = db.Applications.Where(a => a.UserID == userID && a.JobID == id).FirstOrDefault();
            db.Applications.Remove(job);
            db.SaveChanges();
            var jobs = db.Applications.Where(a => a.UserID == userID).OrderByDescending(a => a.Job.PostDate).ToList();
            return PartialView("_Applications", jobs.ToList());
        }

        [Authorize]
        public ActionResult Applicants(Guid id)
        {
            string controller = SecurityUtilities.GetAuthenticatedUserType();
            if (controller != "Employer")
            {
                //redirect to home if user isn't an employer
                return RedirectToAction("Index", controller);
            }
            var all_applicants = db.Applications.Where(a => a.JobID == id).ToList();
            var applicants = all_applicants.Where(a => a.ApplicationStatus == "Accepted").ToList();
            applicants = applicants.Concat( all_applicants.Where(a => a.ApplicationStatus == "Shortlisted")).ToList();
            applicants = applicants.Concat( all_applicants.Where(a => a.ApplicationStatus != "Accepted" && a.ApplicationStatus != "Shortlisted").OrderBy(a => a.ApplicationStatus)).ToList();
            return View(applicants);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditJobStatus(Application application)
        {
            var jobApplication = db.Applications.Where(a => a.JobID == application.JobID && a.UserID == application.UserID).FirstOrDefault();
            jobApplication.ApplicationStatus = application.ApplicationStatus;
            db.SaveChanges();
            return RedirectToAction("Applicants",new { id=application.JobID });
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