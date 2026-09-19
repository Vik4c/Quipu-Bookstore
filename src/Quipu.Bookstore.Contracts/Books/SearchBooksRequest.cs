using Quipu.Bookstore.Domain.Common;

using System.ComponentModel.DataAnnotations;

namespace Quipu.Bookstore.Contracts.Books
{
    public class SearchBooksRequest
    {
        public string? Title { get; init; }

        public string? Author { get; init; }

        [Range(1, int.MaxValue)]
        public int Page { get; init; } = PaginationDefaults.DefaultPage;

        [Range(1, PaginationDefaults.MaxPageSize)]
        public int PageSize { get; init; } = PaginationDefaults.DefaultPageSize;
    }
}
