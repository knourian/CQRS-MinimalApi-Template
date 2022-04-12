using AutoMapper;

using MediatR;

using PEX.Application.Authors.Models;
using PEX.Domain.Model;
using PEX.Infrastructure.Database;

namespace PEX.Application.Authors.Commands;
public static class CreateAuthor
{
	public class Command : IRequest<AuthorGetDto>
	{
		public AuthorDto AuthorDto { get; set; } = default!;
	}

	public class Handler : IRequestHandler<Command, AuthorGetDto>
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public Handler(ApplicationDbContext context, IMapper mapper)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public async Task<AuthorGetDto> Handle(Command request, CancellationToken cancellationToken)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			var toAdd = _mapper.Map<Author>(request.AuthorDto);
			_context.Authors.Add(toAdd);
			await _context.SaveChangesAsync(cancellationToken);
			return _mapper.Map<AuthorGetDto>(toAdd);
		}
	}
}
