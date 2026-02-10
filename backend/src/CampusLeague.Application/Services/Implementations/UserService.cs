using System.Security.Authentication;
using CampusLeague.Application.Extensions;
using CampusLeague.Application.Security;
using CampusLeague.Domain.Entities;
using CampusLeague.Domain.Exceptions;
using CampusLeague.Application.Services.Interfaces;
using CampusLeague.Domain.Params;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CampusLeague.Persistence.Database;

namespace CampusLeague.Application.Services.Implementations;

public class UserService : AbstractEntityService<User>, IUserService
{
    private readonly ApplicationDbContext _dbcontext;
    private readonly IPasswordEncoder _passwordEncoder;
    private readonly ILogger<UserService> _logger;

    public UserService(
        ApplicationDbContext context,
        IPasswordEncoder passwordEncoder,
        ILogger<UserService> logger) : base(context, logger)
    {
        _dbcontext = context;
        _passwordEncoder = passwordEncoder;
        _logger = logger;
    }

    public async Task<IEnumerable<User>> GetAllWithDeletedAsync(
        SortParams? sortParams = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all users including deleted ones...");

        var users = await _dbcontext.Set<User>()
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Sort(sortParams)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Fetched {Count} users (including deleted)", users.Count);
        return users;
    }

    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Searching for user with email {Email}", email);

        var user = await _dbcontext.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User not found with email {Email}", email);
            throw new EntityNotFoundException("User was not found with this email: " + email);
        }

        _logger.LogInformation("User {Id} found by email {Email}", user.Id, email);
        return user;
    }

    public override async Task<User> CreateAsync(
        User user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new user with email {Email}", user.Email);

        user.Password = _passwordEncoder.Encode(user.Password);

        var findUser = await _dbcontext.Set<User>().FirstOrDefaultAsync(
            u => u.Email == user.Email &&
            !u.IsDeleted, cancellationToken);

        if (findUser != null)
        {
            _logger.LogError("Attempt to create user with existing email {Email}", user.Email);
            throw new AuthenticationException("User with this email already exist");
        }

        await _dbcontext.AddAsync(user);
        await _dbcontext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Id} created successfully with email {Email}", user.Id, user.Email);
        return user;
    }

    public async Task SoftDeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Soft deleting user with id {Id}", id);

        var user = await _dbcontext.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User {Id} not found or already deleted", id);
            throw new EntityNotFoundException(nameof(User), id);
        }

        user.IsDeleted = true;
        await _dbcontext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Id} marked as deleted", id);
    }

    public async Task ConfirmEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Confirming email for {Email}", email);

        var user = await _dbcontext.Set<User>()
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User with email {Email} not found for email confirmation", email);
            throw new EntityNotFoundException("User not found with email: " + email);
        }

        if (user.isEmailConfirmed)
        {
            _logger.LogInformation("Email {Email} already confirmed for user {Id}", email, user.Id);
            return;
        }

        user.isEmailConfirmed = true;
        await _dbcontext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Email {Email} successfully confirmed for user {Id}", email, user.Id);
    }

    protected override void UpdateValues(User oldEntity, User newEntity)
    {
        _logger.LogDebug("Updating user {Id} values", oldEntity.Id);

        oldEntity.FirstName = newEntity.FirstName;
        oldEntity.LastName = newEntity.LastName;
        oldEntity.Role = newEntity.Role;
        oldEntity.Email = newEntity.Email;
    }
}
