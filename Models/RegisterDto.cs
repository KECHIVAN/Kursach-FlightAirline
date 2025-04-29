using System.ComponentModel.DataAnnotations;

namespace FlightAirline.Models
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email. Email должен содержать '@' и '.'")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Пароль должен содержать от 6 до 50 символов")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "Пароль должен содержать как минимум одну букву и одну цифру")]
        public string Password { get; set; } = string.Empty;
    }
}