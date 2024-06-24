using System.ComponentModel.DataAnnotations;

namespace WebApii_2.DTO
{
    public class loginUserDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
