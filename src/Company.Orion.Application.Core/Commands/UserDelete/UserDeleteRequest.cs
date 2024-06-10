using MediatR;

namespace Company.Orion.Application.Core.Commands.UserDelete;

public record UserDeleteRequest(string PublicId) : IRequest;
