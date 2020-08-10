using System.ComponentModel.DataAnnotations;

namespace Jobby.Models.Extended
{
    public class UserSkillMetadata{


        [MaxLength(50, ErrorMessage = "Please enter maximum 50 characters. ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required. ")]
        public string SkillName { get; set; }

        public byte SkillLevel { get; set; }

        public bool IsLanguage { get; set; }
    }


}