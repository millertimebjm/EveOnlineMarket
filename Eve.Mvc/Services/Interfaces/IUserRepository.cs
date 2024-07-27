
using Eve.Mvc.Models;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User?> Get(long userId);
    Task<User> Upsert(User user);
}