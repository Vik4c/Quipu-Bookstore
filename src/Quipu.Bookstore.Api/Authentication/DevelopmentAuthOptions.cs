namespace Quipu.Bookstore.Api.Authentication
{
    public class DevelopmentAuthOptions
    {
        public const string SectionName = "DevelopmentAuth";

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? CrudClientSecret { get; set; }
    }
}
