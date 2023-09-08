using MediatR;
using Othala.Users.Events;
using Othala.Users.Models;
using Othala.Users.Repositories;

namespace Othala.Users.Commands;

public record CreateUserCommand(string name, string email, UserStatus status = UserStatus.Active) : IRequest<User>;

internal class CreatedUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;

    public CreatedUserCommandHandler(
        IUserRepository userRepository,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _mediator = mediator;
    }

    public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(request.name)) throw new ArgumentException("Name is required");
        if(string.IsNullOrWhiteSpace(request.email)) throw new ArgumentException("Email is required");

        var user = new User(0, request.name, request.email, request.status);

        user = await _userRepository.AddUser(user);

        await _mediator.Publish(new UserCreatedEvent(user.Id), CancellationToken.None);

        return user;
    }
}