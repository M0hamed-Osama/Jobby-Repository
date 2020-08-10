using System.ComponentModel.DataAnnotations;

namespace Jobby.Models.Extended
{
    public class EmployerProfileViewModel
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

        public byte[] Img { get; set; }
    }
}