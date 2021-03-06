//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Jobby.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Jobby.Models.Extended;

    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.Applications = new HashSet<Application>();
            this.Jobs = new HashSet<Job>();
            this.SavedJobs = new HashSet<SavedJob>();
            this.UserRoles = new HashSet<UserRole>();
            this.UserSkills = new HashSet<UserSkill>();
        }
    
        public System.Guid ID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public byte[] Img { get; set; }
        public string Email { get; set; }
        public string PW { get; set; }
        public System.Guid Code { get; set; }
        public bool IsVerified { get; set; }
        public byte AccessFaildCount { get; set; }
        public Nullable<System.DateTime> LockoutEndDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Application> Applications { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Employer Employer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Job> Jobs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SavedJob> SavedJobs { get; set; }
        public virtual UserEducation UserEducation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRole> UserRoles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserSkill> UserSkills { get; set; }
    }
}
