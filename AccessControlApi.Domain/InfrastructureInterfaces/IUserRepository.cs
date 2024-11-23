using AccessControlApi.Domain.Entities;

namespace AccessControlApi.Domain.InfrastructureInterfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdAsync(int id);
    Task<User> GetByEmailAsync(string email);
    Task<int> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id); 

    Task<User> GetUserByCredentialsAsync(string username, string password);

}
