using MediatR;

namespace Othala.Users.Events;

public record UserCreatedEvent(int userId) : INotification;