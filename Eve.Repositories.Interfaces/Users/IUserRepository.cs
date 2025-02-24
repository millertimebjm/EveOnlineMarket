using Eve.Models.Users;

namespace Eve.Repositories.Interfaces.Users;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User?> Get(long userId);
    Task<User> Upsert(User user);
}