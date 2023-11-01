using System.ComponentModel.DataAnnotations;

namespace ChatApp.Domain.DTOs.Http
{
    public class LoginModelDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
