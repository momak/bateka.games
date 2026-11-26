namespace bateka.games.Api.Identity;

/// <summary>The user <see cref="Middleware.ForwardedUserMiddleware"/> resolved for this request.</summary>
public static class CurrentUserExtensions
{
    private const string ItemKey = "bateka.games.UserId";

    internal static void SetUserId(this HttpContext context, string userId) =>
        context.Items[ItemKey] = userId;

    /// <summary>The owner of everything this request reads or writes (controllers).</summary>
    public static string GetRequiredUserId(this HttpContext context) =>
        context.Items.TryGetValue(ItemKey, out var value) && value is string userId
            ? userId
            : throw new InvalidOperationException("No user on this request — is ForwardedUserMiddleware registered before the endpoints?");
}
