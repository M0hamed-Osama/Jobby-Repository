using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jobby.Models.Extended
{
    public class UserEducationMetadata
    {
        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Degree is required. ")]
        public string Degree { get; set; }

        [MaxLength(100, ErrorMessage = "Please enter maximum 100 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Entity is required. ")]
        public string Entity { get; set; }

        [MaxLength(100, ErrorMessage = "Please enter maximum 100 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Field is required. ")]
        public string Field { get; set; }

        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Grade is required. ")]
        public string Grade { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Start Year is required. ")]
        [StringLength(4, ErrorMessage = "Please enter 4 digits only. ")]
        public string StartYear { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "End Year is required. ")]
        [StringLength(4, ErrorMessage = "Please enter 4 digits only. ")]
        public string EndYear { get; set; }
    }
}