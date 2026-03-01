using InsurancePlatform.Api.Domain;

namespace InsurancePlatform.Api.Security;

public static class HttpContextExtensions
{
    public const string CurrentUserItemKey = "CurrentUser";

    public static UserAccount GetCurrentUser(this HttpContext context)
    {
        if (context.Items.TryGetValue(CurrentUserItemKey, out var user) && user is UserAccount currentUser)
        {
            return currentUser;
        }

        throw new InvalidOperationException("Authenticated user is not available in the current request.");
    }
}
