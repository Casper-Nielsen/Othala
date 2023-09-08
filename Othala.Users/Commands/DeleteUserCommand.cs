using MediatR;
using Othala.Users.Events;
using Othala.Users.Models;
using Othala.Users.Repositories;

namespace Othala.Users.Commands;

public record DeleteUserCommand(int userId) : IRequest;

internal class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;
    
    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _mediator = mediator;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        if(request.userId <= 0) throw new ArgumentException("userId cannot be less than or equal to 0");
        
        await _userRepository.UpdateUserStatus(request.userId, UserStatus.Deleted);

        await _mediator.Publish(new UserDeletedEvent(request.userId), CancellationToken.None);
    }
}