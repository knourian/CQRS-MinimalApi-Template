using AutoMapper;

using MediatR;

using PEX.Application.Authors.Models;
using PEX.Domain.Model;
using PEX.Infrastructure.Database;
using PEX.Infrastructure.Repository.Contracts;

namespace PEX.Application.Authors.Commands;
public static class CreateAuthor
{
    public class Command : IRequest<AuthorGetDto>
    {
        public AuthorDto AuthorDto { get; set; } = default!;
    }

    public class Handler : IRequestHandler<Command, AuthorGetDto>
    {
        private readonly IAuthorRepository _repository;
        private readonly IMapper _mapper;

        public Handler(IMapper mapper, IAuthorRepository repository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository;
        }

        public async Task<AuthorGetDto> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var toAdd = _mapper.Map<Author>(request.AuthorDto);
            await _repository.AddAsync(toAdd, cancellationToken);
            return _mapper.Map<AuthorGetDto>(toAdd);
        }
    }
}
