using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Website.Classes
{
    public class NameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(Regex.IsMatch((string)value, @"^\b[a-zA-Z\s]{1,40}\b$"))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid name.");
        }
    }
}
