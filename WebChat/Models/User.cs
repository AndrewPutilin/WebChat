using Microsoft.AspNetCore.Identity;

namespace WebChat.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
    }
}
