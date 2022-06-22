using FluentValidation;

using PEX.Application.Authors.Models;

namespace PEX.Api.Validations;
public class AuthorValidator : AbstractValidator<AuthorDto>
{
    public AuthorValidator(ILogger<AuthorValidator> logger)
    {
        RuleFor(author => author.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(author => author.LastName).NotEmpty();
    }
}
