using TravelDNA.Core.Models;

namespace TravelDNA.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> FindByGoogleSubjectIdAsync(string googleSubjectId, CancellationToken cancellationToken = default);
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}