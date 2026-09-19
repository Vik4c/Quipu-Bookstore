namespace Quipu.Bookstore.Contracts.Authors
{
    public class AuthorResponse
    {
        public int AuthorId { get; init; }

        public required string Name { get; init; }
    }
}
