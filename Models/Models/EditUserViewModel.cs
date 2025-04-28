using System.ComponentModel.DataAnnotations;

namespace Server.Areas.Admin.ViewModels
{
    public class EditUserViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required, Display(Name = "Username")]
        public string UserName { get; set; }

        [Required, EmailAddress, Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
