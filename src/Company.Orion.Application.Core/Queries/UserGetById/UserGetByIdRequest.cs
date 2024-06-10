using MediatR;

namespace Company.Orion.Application.Core.Queries.UserGetById;

public record UserGetByIdRequest(string PublicId) : IRequest<UserGetByIdResponse>;
