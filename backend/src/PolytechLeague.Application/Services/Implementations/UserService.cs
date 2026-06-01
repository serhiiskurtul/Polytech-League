using System.Security.Authentication;
using PolytechLeague.Application.Extensions;
using PolytechLeague.Application.Security;
using PolytechLeague.Domain.Entities;
using PolytechLeague.Domain.Exceptions;
using PolytechLeague.Application.Services.Interfaces;
using PolytechLeague.Domain.Params;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PolytechLeague.Persistence.Database;

namespace PolytechLeague.Application.Services.Implementations;

public class UserService : AbstractEntityService<User>, IUserService
{
    private readonly IPasswordEncoder _passwordEncoder;

    public UserService(
        ApplicationDbContext context,
        IPasswordEncoder passwordEncoder,
        ILogger<UserService> logger) : base(context, logger)
    {
        _passwordEncoder = passwordEncoder;
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
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower(), cancellationToken);

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

        if (user.Email != null)
        {
            var findUser = await _dbcontext.Set<User>().FirstOrDefaultAsync(
                u => u.Email != null && u.Email.ToLower() == user.Email.ToLower() &&
                !u.IsDeleted, cancellationToken);

            if (findUser != null)
            {
                _logger.LogError("Attempt to create user with existing email {Email}", user.Email);
                throw new AuthenticationException("User with this email already exist");
            }
        }

        await _dbcontext.AddAsync(user);
        await _dbcontext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Id} created successfully with email {Email}", user.Id, user.Email);
        return user;
    }

    public override async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Hard deleting user with id {Id} and all associated data", id);

        var user = await _dbcontext.Set<User>().FindAsync(new object[] { id }, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {Id} not found for hard delete", id);
            throw new EntityNotFoundException(nameof(User), id);
        }

        // Delete Conversations and Messages
        var userConversations = await _dbcontext.Set<Conversation>()
            .Where(c => c.User1Id == id || c.User2Id == id)
            .ToListAsync(cancellationToken);

        if (userConversations.Any())
        {
            var conversationIds = userConversations.Select(c => c.Id).ToList();
            var conversationMessages = await _dbcontext.Set<Message>()
                .Where(m => conversationIds.Contains(m.ConversationId))
                .ToListAsync(cancellationToken);
            
            if (conversationMessages.Any())
                _dbcontext.Set<Message>().RemoveRange(conversationMessages);

            _dbcontext.Set<Conversation>().RemoveRange(userConversations);
        }

        // Also delete any remaining messages sent by user in other conversations
        var otherMessages = await _dbcontext.Set<Message>()
            .Where(m => m.SenderId == id)
            .ToListAsync(cancellationToken);
        if (otherMessages.Any())
            _dbcontext.Set<Message>().RemoveRange(otherMessages);

        // Delete ProfileMergeRequests
        var userMergeRequests = await _dbcontext.Set<ProfileMergeRequest>()
            .Where(r => r.UserId == id)
            .ToListAsync(cancellationToken);
        if (userMergeRequests.Any())
            _dbcontext.Set<ProfileMergeRequest>().RemoveRange(userMergeRequests);

        var persons = await _dbcontext.Set<Person>()
            .Where(p => p.UserId == id)
            .ToListAsync(cancellationToken);

        if (persons.Any())
        {
            _logger.LogInformation("Deleting {Count} associated person profiles for hard deleted user {UserId}", persons.Count, id);
            _dbcontext.Set<Person>().RemoveRange(persons);
        }

        var conversations = await _dbcontext.Set<Conversation>()
            .Where(c => c.User1Id == id || c.User2Id == id)
            .ToListAsync(cancellationToken);
        if (conversations.Any())
        {
            var conversationIds = conversations.Select(c => c.Id).ToList();
            var messages = await _dbcontext.Set<Message>()
                .Where(m => conversationIds.Contains(m.ConversationId) || m.SenderId == id)
                .ToListAsync(cancellationToken);
            if (messages.Any())
                _dbcontext.Set<Message>().RemoveRange(messages);
            _dbcontext.Set<Conversation>().RemoveRange(conversations);
        }

        var notifications = await _dbcontext.Set<Notification>()
            .Where(n => n.UserId == id)
            .ToListAsync(cancellationToken);
        if (notifications.Any())
            _dbcontext.Set<Notification>().RemoveRange(notifications);

        var pushSubs = await _dbcontext.Set<PushSubscription>()
            .Where(p => p.UserId == id)
            .ToListAsync(cancellationToken);
        if (pushSubs.Any())
            _dbcontext.Set<PushSubscription>().RemoveRange(pushSubs);

        var mergeReqs = await _dbcontext.Set<ProfileMergeRequest>()
            .Where(r => r.UserId == id)
            .ToListAsync(cancellationToken);
        if (mergeReqs.Any())
            _dbcontext.Set<ProfileMergeRequest>().RemoveRange(mergeReqs);

        var eventVotes = await _dbcontext.Set<EventVote>()
            .Where(e => e.UserId == id)
            .ToListAsync(cancellationToken);
        if (eventVotes.Any())
            _dbcontext.Set<EventVote>().RemoveRange(eventVotes);

        var feedbacks = await _dbcontext.Set<Feedback>()
            .Where(f => f.UserId == id)
            .ToListAsync(cancellationToken);
        if (feedbacks.Any())
            _dbcontext.Set<Feedback>().RemoveRange(feedbacks);

        var media = await _dbcontext.Set<UserEventMedia>()
            .Where(m => m.UserId == id)
            .ToListAsync(cancellationToken);
        if (media.Any())
            _dbcontext.Set<UserEventMedia>().RemoveRange(media);

        _dbcontext.Set<User>().Remove(user);
        await _dbcontext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Id} and associated data successfully deleted", id);
    }

    public async Task SoftDeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Soft deleting user with id {Id} and all associated person profiles", id);

        var user = await _dbcontext.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User {Id} not found or already deleted", id);
            throw new EntityNotFoundException(nameof(User), id);
        }

        var persons = await _dbcontext.Set<Person>()
            .Where(p => p.UserId == id)
            .ToListAsync(cancellationToken);

        if (persons.Any())
        {
            _logger.LogInformation("Deleting {Count} associated person profiles for soft deleted user {UserId}", persons.Count, id);
            _dbcontext.Set<Person>().RemoveRange(persons);
        }

        user.IsDeleted = true;
        await _dbcontext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Id} marked as deleted", id);
    }

    public async Task BlockAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Blocking user with id {Id}", id);

        var user = await _dbcontext.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User {Id} not found or already deleted", id);
            throw new EntityNotFoundException(nameof(User), id);
        }

        user.IsBlocked = true;
        await _dbcontext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Id} blocked", id);
    }

    public async Task UnblockAsync(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Unblocking user with id {Id}", id);

        var user = await _dbcontext.Set<User>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User {Id} not found or already deleted", id);
            throw new EntityNotFoundException(nameof(User), id);
        }

        user.IsBlocked = false;
        await _dbcontext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Id} unblocked", id);
    }

    public async Task ConfirmEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Confirming email for {Email}", email);

        var user = await _dbcontext.Set<User>()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && !u.IsDeleted, cancellationToken);

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
        oldEntity.PhotoUrl = newEntity.PhotoUrl;
        oldEntity.IsBlocked = newEntity.IsBlocked;
    }

    protected override async Task OnUpdatedAsync(User oldEntity, User newEntity, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Syncing updated name/photo from user {UserId} to persons", oldEntity.Id);

        var persons = await _dbcontext.Set<Person>()
            .Where(p => p.UserId == oldEntity.Id)
            .ToListAsync(cancellationToken);

        if (persons.Any())
        {
            foreach (var person in persons)
            {
                person.FirstName = oldEntity.FirstName;
                person.LastName = oldEntity.LastName;
                if (!string.IsNullOrEmpty(oldEntity.PhotoUrl))
                {
                    person.PhotoUrl = oldEntity.PhotoUrl;
                }
            }

            _dbcontext.Set<Person>().UpdateRange(persons);
            await _dbcontext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully synced user {UserId} changes to {Count} persons", oldEntity.Id, persons.Count);
        }
    }
}
