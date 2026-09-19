using Quipu.Bookstore.Domain.Common;
using MediatR;

namespace Quipu.Bookstore.Application.Abstractions
{
    public interface IRequestWithResultHandler<TRequest, TResponse>
        : IRequestHandler<TRequest, TResponse> where TRequest : IRequestWithResult<TResponse> where TResponse : Result
    { }
}
