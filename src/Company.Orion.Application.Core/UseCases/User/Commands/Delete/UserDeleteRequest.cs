using MediatR;

namespace Company.Orion.Application.Core.UseCases.User.Commands.Delete;

public record UserDeleteRequest(string PublicId) : IRequest;
