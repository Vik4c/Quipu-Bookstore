using Quipu.Bookstore.Api.Authorization;
using Quipu.Bookstore.Api.Mappings;
using Quipu.Bookstore.Application.Books;
using Quipu.Bookstore.Application.Books.CreateBook;
using Quipu.Bookstore.Application.Books.DeleteBook;
using Quipu.Bookstore.Application.Books.GetBookById;
using Quipu.Bookstore.Application.Books.SearchBooks;
using Quipu.Bookstore.Application.Books.UpdateBook;
using Quipu.Bookstore.Contracts.Books;
using Quipu.Bookstore.Contracts.Common;
using Quipu.Bookstore.Domain.Common;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Quipu.Bookstore.Api.Controllers
{
    [Route("api/books")]
    public class BooksController(IMediator _mediator) : BaseController
    {
        [HttpGet("{id:int}")]
        [Authorize(Policy = BookstorePolicies.BooksManage)]
        [ProducesResponseType<BookResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            GetBookByIdQuery query = new(bookId: id);

            Result<BookDto> result = await _mediator.Send(query, cancellationToken);
            return Result(result, book => Ok(book.ToBookResponse()));
        }

        [HttpGet]
        [Authorize(Policy = BookstorePolicies.BooksRead)]
        [ProducesResponseType<PagedResponse<BookResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search(
            [FromQuery] SearchBooksRequest request,
            CancellationToken cancellationToken)
        {
            SearchBooksQuery query = new(
                title: request.Title,
                author: request.Author,
                page: request.Page,
                pageSize: request.PageSize
            );

            Result<PagedResult<BookDto>> result = await _mediator.Send(query, cancellationToken);
            return Result(result, page => Ok(page.ToPagedBookResponse()));
        }

        [HttpPost]
        [Authorize(Policy = BookstorePolicies.BooksManage)]
        [ProducesResponseType<BookResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateBookRequest request,
            CancellationToken cancellationToken)
        {
            CreateBookCommand command = new(
                title: request.Title,
                subTitle: request.SubTitle,
                authorId: request.AuthorId
            );

            Result<BookDto> result = await _mediator.Send(command, cancellationToken);
            return Result(result, book => CreatedAtAction(nameof(GetById), new { id = book.BookId }, book.ToBookResponse()));
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = BookstorePolicies.BooksManage)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            [FromRoute] int id,
            [FromBody] UpdateBookRequest request,
            CancellationToken cancellationToken)
        {
            UpdateBookCommand command = new(
                bookId: id,
                title: request.Title,
                subTitle: request.SubTitle,
                authorId: request.AuthorId
            );

            Result result = await _mediator.Send(command, cancellationToken);
            return Result(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = BookstorePolicies.BooksManage)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            DeleteBookCommand command = new(bookId: id);

            Result result = await _mediator.Send(command, cancellationToken);
            return Result(result);
        }
    }
}
