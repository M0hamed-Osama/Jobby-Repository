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
    
    public partial class UserRole
    {
        public System.Guid UserID { get; set; }
        public System.Guid RoleID { get; set; }
        public string Notes { get; set; }
    
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
