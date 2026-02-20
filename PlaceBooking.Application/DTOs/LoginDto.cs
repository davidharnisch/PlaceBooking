using System.ComponentModel.DataAnnotations;

namespace PlaceBooking.Application.DTOs;

public class LoginDto
{
    [Display(Name = "Email")]
    [Required(ErrorMessage = "Vyplňte email.")]
    [EmailAddress(ErrorMessage = "Zadejte platný email (např. jmeno@domena.cz).")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Heslo")]
    [Required(ErrorMessage = "Vyplňte heslo.")]
    public string Password { get; set; } = string.Empty;
}
