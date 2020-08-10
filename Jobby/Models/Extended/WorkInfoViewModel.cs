using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Jobby.Models.Extended
{
    public class WorkInfoViewModel
    {
        public Nullable<int> Salary { get; set; }
        public string JobStatus { get; set; }
        public string CareerLevel { get; set; }
        public HttpPostedFileBase CV { get; set; }
    }
}