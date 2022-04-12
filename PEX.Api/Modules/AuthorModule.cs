using MediatR;
using FluentValidation;
using PEX.Application.Authors.Commands;
using PEX.Application.Authors.Models;
using PEX.Application.Authors.Queries;
using PEX.Application.Contracts;
using PEX.Api.Validations;
using PEX.Api.Extensions;

namespace PEX.Api.Modules;
public class AuthorModule : IModule
{
	public IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/api/authors", GetAllAuthorsAsync)
			.WithName("GetAllAuthors")
			.WithDisplayName("Authors")
			.WithTags("Authors")
			.Produces<List<AuthorGetDto>>()
			.Produces(500);

		endpoints.MapPost("/api/authors", CreateAuthorAsync)
			.WithName("CreateAuthor")
			.WithDisplayName("Authors")
			.WithTags("Authors")
			.WithValidator<AuthorDto>()
			.Produces<AuthorGetDto>()
			.Produces(500);

		endpoints.MapGet("api/authors/{id}", GetAuthorById)
			.WithName("GetAuthorById")
			.WithDisplayName("Authors")
			.Produces<AuthorGetDto>()
			.Produces(404)
			.Produces(500);

		endpoints.MapPut("api/authors/{id}", UpdateAuthor)
			.WithName("UpdateAuthor")
			.WithDisplayName("Authors")
			.WithValidator<AuthorDto>()
			.Produces(204)
			.Produces(500);

		endpoints.MapDelete("api/authors/{id}", DeleteAuthor)
			.WithName("DeleteAuthor")
			.WithDisplayName("Authors")
			.Produces(204)
			.Produces(500);

		return endpoints;
	}
	private static async Task<IResult> CreateAuthorAsync(AuthorDto authorDto, IMediator mediator, CancellationToken ct)
	{

		var command = new CreateAuthor.Command { AuthorDto = authorDto };
		var author = await mediator.Send(command, ct);
		return Results.Ok(author);
	}

	private static async Task<IResult> GetAllAuthorsAsync(IMediator mediator, CancellationToken ct)
	{
		var request = new GetAllAuthors.Query();
		var authors = await mediator.Send(request, ct);
		return Results.Ok(authors);
	}

	private async Task<IResult> GetAuthorById(int id, IMediator mediator, CancellationToken ct)
	{
		var result = await mediator.Send(new GetAuthorById.Query { AuthorId = id }, ct);
		if (result is null)
		{
			return Results.NotFound();
		}

		return Results.Ok(result);
	}

	private async Task<IResult> UpdateAuthor(int id, AuthorDto authorToUpdate, IMediator mediator, CancellationToken ct)
	{
		var command = new UpdateAuthor.Command
		{
			AuthorId = id,
			FirstName = authorToUpdate.FirstName,
			LastName = authorToUpdate.LastName,
			Bio = authorToUpdate.Bio ?? "",
			DateOfBirth = authorToUpdate.DateOfBirth
		};
		await mediator.Send(command, ct);

		return Results.NoContent();
	}

	private async Task<IResult> DeleteAuthor(int id, IMediator mediator, CancellationToken ct)
	{
		await mediator.Send(new DeleteAuthor.Command { AuthorId = id }, ct);
		return Results.NoContent();
	}
}