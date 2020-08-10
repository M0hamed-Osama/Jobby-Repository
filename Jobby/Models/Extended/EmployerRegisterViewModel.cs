using System.ComponentModel.DataAnnotations;

namespace Jobby.Models.Extended
{
    public class EmployerRegisterViewModel
    {
        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required. ")]
        public string FName { get; set; }

        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required. ")]
        public string LName { get; set; }

        [MaxLength(100, ErrorMessage = "Please enter maximum 100 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Company name is required. ")]
        public string CName { get; set; }

        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Industry is required. ")]
        public string Industry { get; set; }

        [MaxLength(100, ErrorMessage = "Please enter maximum 100 characters. ")]
        public string Website { get; set; }

        [Display(Name = "Email")]
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