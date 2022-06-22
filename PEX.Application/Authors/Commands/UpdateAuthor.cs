
using MediatR;

using Microsoft.EntityFrameworkCore;

using PEX.Infrastructure.Database;
using PEX.Infrastructure.Repository.Contracts;

namespace PEX.Application.Authors.Commands;
public static class UpdateAuthor
{
    public class Command : IRequest
    {
        public int AuthorId { get; init; }
        public string FirstName { get; init; } = default!;
        public string LastName { get; init; } = default!;
        public string? Bio { get; init; }
        public DateTime DateOfBirth { get; init; }
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

            if (author is not null)
            {
                author.FirstName = request.FirstName;
                author.LastName = request.LastName;
                author.Bio = request.Bio;
                author.DateOfBirth = request.DateOfBirth;
            }

            await _repository.UpdateAsync(author!.Id, author);
            return Unit.Value;
        }
    }
}
