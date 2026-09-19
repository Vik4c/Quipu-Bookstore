namespace Quipu.Bookstore.Domain.Entities
{
    public class Book
    {
        public const int TitleMinLength = 3;

        public const int TitleMaxLength = 100;

        public int BookId { get; private set; }

        public string Title { get; private set; } = null!;

        public string? SubTitle { get; private set; }

        public int AuthorId { get; private set; }

        public Author Author { get; private set; } = null!;

        private Book()
        {
        }

        public Book(string title, string? subTitle, int authorId)
        {
            SetDetails(
                title: title,
                subTitle: subTitle,
                authorId: authorId
            );
        }

        public void UpdateDetails(string title, string? subTitle, int authorId)
        {
            SetDetails(
                title: title,
                subTitle: subTitle,
                authorId: authorId
            );
        }

        private void SetDetails(string title, string? subTitle, int authorId)
        {
            if (string.IsNullOrWhiteSpace(title) || title.Length < TitleMinLength || title.Length > TitleMaxLength)
                throw new ArgumentException(
                    $"Book title must be between {TitleMinLength} and {TitleMaxLength} characters.",
                    nameof(title)
                );

            if (authorId <= 0)
                throw new ArgumentException("Book must reference a valid author.", nameof(authorId));

            Title = title;
            SubTitle = subTitle;
            AuthorId = authorId;
        }
    }
}
