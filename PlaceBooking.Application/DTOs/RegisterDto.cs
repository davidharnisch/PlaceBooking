using System.ComponentModel.DataAnnotations;

namespace PlaceBooking.Application.DTOs;

public class RegisterDto
{
    [Display(Name = "Jméno")]
    [Required(ErrorMessage = "Vyplòte jméno.")]
    public string FirstName { get; set; } = string.Empty;
    
    [Display(Name = "Pøíjmení")]
    [Required(ErrorMessage = "Vyplòte pøíjmení.")]
    public string LastName { get; set; } = string.Empty;
    
    [Display(Name = "Email")]
    [Required(ErrorMessage = "Vyplòte email.")]
    [EmailAddress(ErrorMessage = "Zadejte platný email (napø. jmeno@domena.cz).")]
    public string Email { get; set; } = string.Empty;
    
    [Display(Name = "Heslo")]
    [Required(ErrorMessage = "Vyplòte heslo.")]
    [MinLength(6, ErrorMessage = "Heslo musí mít alespoò 6 znakù")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Potvrzení hesla")]
    [Required(ErrorMessage = "Vyplòte potvrzení hesla.")]
    [Compare("Password", ErrorMessage = "Hesla se musí shodovat")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
