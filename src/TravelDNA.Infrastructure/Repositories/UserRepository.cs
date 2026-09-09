using Microsoft.EntityFrameworkCore;
using TravelDNA.Core.Interfaces;
using TravelDNA.Core.Models;
using TravelDNA.Infrastructure.Data;

namespace TravelDNA.Infrastructure.Repositories;

public sealed class UserRepository(TravelDnaDbContext dbContext) : IUserRepository
{
    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
        return user;
    }

    public Task<User?> FindByGoogleSubjectIdAsync(string googleSubjectId, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.SingleOrDefaultAsync(user => user.GoogleSubjectId == googleSubjectId, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}