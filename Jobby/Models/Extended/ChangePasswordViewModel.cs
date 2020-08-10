using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jobby.Models.Extended
{
    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Please enter at least 6 characters. ")]
        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Current Password is required. ")]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Please enter at least 6 characters. ")]
        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "New Password is required. ")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password does not match. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm Password is required. ")]
        public string ConfirmPassword { get; set; }
    }
}