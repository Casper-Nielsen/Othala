using MediatR;
using Othala.Users.Events;
using Othala.Users.Models;
using Othala.Users.Repositories;

namespace Othala.Users.Commands;

public record AddUserRoleCommand(int UserId, UserRole Role) : IRequest;

internal class AddUserRoleCommandHandler : IRequestHandler<AddUserRoleCommand>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IMediator _mediator;

    public AddUserRoleCommandHandler(
        IUserRoleRepository userRoleRepository,
        IMediator mediator)
    {
        _userRoleRepository = userRoleRepository;
        _mediator = mediator;
    }

    public async Task Handle(AddUserRoleCommand request, CancellationToken cancellationToken)
    {
        var userRoles = await _userRoleRepository.GetUserRoles(request.UserId);

        if (userRoles.Any(x => x == request.Role)) return;
        
        await _userRoleRepository.AddUserRole(request.UserId, request.Role);

        await _mediator.Publish(new ChangedUserEvent(request.UserId), CancellationToken.None);
    }
}