using MediatR;

namespace Othala.Users.Events;

public record UserDeletedEvent (int userId) : INotification;