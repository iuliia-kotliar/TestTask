namespace API_Assessment.Models
{
    public class TokenModel
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public string DisplayName { get; set; }
        public string Issued { get; set; }
        public string Expires { get; set; }
    }
}
