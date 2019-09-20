namespace Website.Classes
{
    public struct TokenData
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string AccessTokenExpiration { get; set; }
        public CustomerDTO Customer { get; set; }
    }
}
