using MediatR;
using Othala.Users.Events;
using Othala.Users.Models;
using Othala.Users.Repositories;

namespace Othala.Users.Commands;

public record RemoveUserRoleCommand(int UserId, UserRole Role) : IRequest;

internal class RemoveUserRoleCommandHandler : IRequestHandler<RemoveUserRoleCommand>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IMediator _mediator;

    public RemoveUserRoleCommandHandler(
        IUserRoleRepository userRoleRepository,
        IMediator mediator)
    {
        _userRoleRepository = userRoleRepository;
        _mediator = mediator;
    }

    public async Task Handle(RemoveUserRoleCommand request, CancellationToken cancellationToken)
    {
        var userRoles = await _userRoleRepository.GetUserRoles(request.UserId);
        
        if (userRoles.All(x => x != request.Role)) return;
        
        await _userRoleRepository.RemoveUserRole(request.UserId, request.Role);

        await _mediator.Publish(new ChangedUserEvent(request.UserId), CancellationToken.None);
    }
}