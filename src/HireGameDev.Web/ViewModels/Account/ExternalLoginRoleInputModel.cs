using System.ComponentModel.DataAnnotations;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Web.ViewModels.Account;

public sealed class ExternalLoginRoleInputModel
{
    [Required(ErrorMessage = "Please choose whether you are a game developer or a studio.")]
    [Display(Name = "I am a")]
    public UserRole? Role { get; set; }
}
