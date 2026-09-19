namespace Quipu.Bookstore.Domain.Entities
{
    public class Author
    {
        public const int NameMinLength = 3;

        public const int NameMaxLength = 100;

        public int AuthorId { get; private set; }

        public string Name { get; private set; } = null!;

        private Author()
        {
        }

        public Author(string name)
        {
            SetName(name);
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < NameMinLength || name.Length > NameMaxLength)
                throw new ArgumentException(
                    $"Author name must be between {NameMinLength} and {NameMaxLength} characters.",
                    nameof(name)
                );

            Name = name;
        }
    }
}
