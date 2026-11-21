using Microsoft.AspNetCore.Identity;

namespace MyLibrary.Infrastructure.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsPasswordResetRequired { get; set; } = true;
    }
}
