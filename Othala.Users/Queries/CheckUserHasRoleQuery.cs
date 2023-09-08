using MediatR;
using Othala.Users.Models;
using Othala.Users.Repositories;

namespace Othala.Users.Queries;

public record CheckUserHasRoleQuery(int UserId, UserRole Role) : IRequest<bool>;

internal class CheckUserHasRoleHandler : IRequestHandler<CheckUserHasRoleQuery, bool>
{
    private readonly IUserRoleRepository _userRoleRepository;

    public CheckUserHasRoleHandler(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public async Task<bool> Handle(CheckUserHasRoleQuery request, CancellationToken cancellationToken)
    {
        var userRoles = await _userRoleRepository.GetUserRoles(request.UserId, true);

        return userRoles.Any(x => x == request.Role);
    }
}