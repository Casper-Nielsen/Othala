using MediatR;
using Othala.Users.Models;
using Othala.Users.Repositories;

namespace Othala.Users.Queries;

public record GetUserById(int userId) : IRequest<User>;

internal class GetUserByIdHandler : IRequestHandler<GetUserById, User>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public GetUserByIdHandler(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<User> Handle(GetUserById request, CancellationToken cancellationToken)
    {
        if(request.userId <= 0) throw new ArgumentException("userId cannot be less than or equal to 0");

        var user = await _userRepository.GetUser(request.userId);
        
        if(user is null) throw new ArgumentException("User not found");

        user.UserRoles = (await _userRoleRepository.GetUserRoles(user.Id)).ToList();

        return user;
    }
}