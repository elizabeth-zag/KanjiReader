using KanjiReader.Infrastructure.Database.Models;

namespace KanjiReader.Domain.UserAccount;

public record struct LogInResult(User? User, bool NeedEmailConfirmation);