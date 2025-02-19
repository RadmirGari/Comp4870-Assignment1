namespace Assignment1.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
public class User : IdentityUser
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Contributor";

    public bool Approved { get; set; } = false;
}

