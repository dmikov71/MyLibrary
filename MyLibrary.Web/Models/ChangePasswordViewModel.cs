using System.ComponentModel.DataAnnotations;

namespace MyLibrary.Web.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Требуется текущий пароль")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Требуется новый пароль")]
        [StringLength(100, ErrorMessage = "Пароль должен быть не менее {2} символов", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

}
