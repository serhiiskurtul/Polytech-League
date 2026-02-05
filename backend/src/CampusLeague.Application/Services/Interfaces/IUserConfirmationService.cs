
public interface IUserConfirmationService
{
    Task ConfirmEmailAsync(string email, CancellationToken cancellationToken = default);
}