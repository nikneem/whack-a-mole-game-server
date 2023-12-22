using Wam.Users.DomainModels;

namespace Wam.Users.Repositories;

public interface IUsersRepository
{
    Task<bool> Save(User user, CancellationToken cancellationToken);
    Task<User> Get(Guid userId, CancellationToken cancellationToken);
}