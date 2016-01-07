using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Unosquare.PassCore.Web.Models
{
    public class ChangePasswordModel
    {
        [Required, Display(Name="User Principal Name")]
        public string UserPrincipalName { get; set; }
        [Required, Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }
        [Required, Display(Name = "New Password")]
        public string NewPassword { get; set; }
        [Compare("NewPassword"), Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}