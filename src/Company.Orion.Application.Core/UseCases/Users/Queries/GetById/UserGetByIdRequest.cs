using MediatR;

namespace Company.Orion.Application.Core.UseCases.Users.Queries.GetById;

public record UserGetByIdRequest(string PublicId) : IRequest<UserGetByIdResponse>;
