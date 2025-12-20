using KanjiReader.Infrastructure.Database.Models;

namespace KanjiReader.Domain.UserAccount;

public record struct LogInResult(User? User, string? ErrorMessage, bool NeedEmailConfirmation);