using System.ComponentModel.DataAnnotations;

namespace Jobby.Models.Extended
{
    public class ResetPasswordViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Code is required. ")]
        public string Code { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Please enter at least 6 characters. ")]
        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required. ")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm Password does not match. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required. ")]
        public string ConfirmPassword { get; set; }
    }
}