using System.ComponentModel.DataAnnotations;

namespace Jobby.Models.Extended
{
    public class JobMetadata
    {
        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required. ")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is required. ")]
        public string JobDesc { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Country is required. ")]
        public int CountryID { get; set; }

        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "City is required. ")]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Salary is required. ")]
        public int Salary { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Career Level is required. ")]
        public string CareerLevel { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Job Field is required. ")]
        public int JobFieldID { get; set; }
    }
}