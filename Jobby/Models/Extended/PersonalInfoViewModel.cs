using System;
using System.ComponentModel.DataAnnotations;

namespace Jobby.Models.Extended
{
    public class PersonalInfoViewModel
    {

        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required. ")]
        public string FName { get; set; }

        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required. ")]
        public string LName { get; set; }

        public Nullable<int> CountryID { get; set; }

        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        public string City { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> BirthDate { get; set; }

        [MaxLength(20, ErrorMessage = "Please enter maximum 20 characters. ")]
        public string PhoneNumber { get; set; }
        
        public string MartialStatus { get; set; }
        
        public string MilitaryStatus { get; set; }

    }
}