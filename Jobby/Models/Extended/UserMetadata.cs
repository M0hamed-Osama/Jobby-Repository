using System.ComponentModel.DataAnnotations;

namespace Jobby.Models.Extended
{
    public class UserMetadata{

        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required. ")]
        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        public string FName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required. ")]
        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        public string LName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required. ")]
        [MaxLength(320, ErrorMessage = "Please enter maximum 320 characters. ")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Please enter at least 6 characters. ")]
        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required. ")]
        public string PW { get; set; }

    }


}