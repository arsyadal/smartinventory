using SmartInventory.Domain.Entities;

namespace SmartInventory.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
}
