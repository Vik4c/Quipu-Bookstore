using Quipu.Bookstore.Domain.Common;
using MediatR;

namespace Quipu.Bookstore.Application.Abstractions
{
    public interface IRequestWithResult<TResponse> : IRequest<TResponse> where TResponse : Result { }
}
