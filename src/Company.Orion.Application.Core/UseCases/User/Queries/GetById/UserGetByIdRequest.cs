using MediatR;

namespace Company.Orion.Application.Core.UseCases.User.Queries.GetById;

public record UserGetByIdRequest(string PublicId) : IRequest<UserGetByIdResponse>;
