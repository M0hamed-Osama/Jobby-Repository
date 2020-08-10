using System.ComponentModel.DataAnnotations;

namespace Jobby.Models.Extended
{
    public class LoginViewModel
    {
        [Display(Name ="Email")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(320, ErrorMessage = "Please enter maximum 320 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required. ")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Please enter at least 6 characters. ")]
        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required. ")]
        public string PW { get; set; }
    }
}