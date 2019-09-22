using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Website.Classes
{
    public class PasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (Regex.IsMatch((string)value, @"(?=.*\S).{6,}"))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Passwords must be at least 6 characters.");
        }
    }
}
