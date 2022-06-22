
using MediatR;

using Microsoft.EntityFrameworkCore;

using PEX.Infrastructure.Database;
using PEX.Infrastructure.Repository.Contracts;

namespace PEX.Application.Authors.Commands;
public static class DeleteAuthor
{
    public class Command : IRequest
    {
        public int AuthorId { get; init; }
    }

    public class Handler : IRequestHandler<Command>
    {

        private readonly IAuthorRepository _repository;

        public Handler(IAuthorRepository repository)
        {

            _repository = repository;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var author = await _repository.GetAsync(request.AuthorId);
            if (author != null)
            {
                await _repository.RemoveAsync(author);
            }
            return Unit.Value;
        }
    }
}