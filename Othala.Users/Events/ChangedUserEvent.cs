using MediatR;

namespace Othala.Users.Events;

public record ChangedUserEvent(int userId) : INotification;