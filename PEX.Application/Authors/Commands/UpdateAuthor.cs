
using MediatR;

using Microsoft.EntityFrameworkCore;

using PEX.Infrastructure.Database;

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
        private readonly ApplicationDbContext _context;


        public Handler(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == request.AuthorId, cancellationToken);
            if (author is not null)
            {
                author.FirstName = request.FirstName;
                author.LastName = request.LastName;
                author.Bio = request.Bio;
                author.DateOfBirth = request.DateOfBirth;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
