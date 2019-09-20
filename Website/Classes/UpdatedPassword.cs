namespace Website.Classes
{
    public struct UpdatedPassword
    {
        public string CurrentPassword { get; set; }
        [Password]
        public string NewPassword { get; set; }
    }
}
