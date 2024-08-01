using MediatR;

namespace Company.Orion.Application.Core.UseCases.Users.Commands.Delete;

public record UserDeleteRequest(string PublicId) : IRequest;
