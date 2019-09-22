using System.ComponentModel.DataAnnotations;

namespace Website.Classes
{
    public struct UpdatedPassword
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Password]
        public string NewPassword { get; set; }
    }
}
