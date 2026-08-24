
namespace TmsApi.Api.RateLimiting;

public enum ApiKeyTier
{
    Anonymous = 0,
    Free = 1,
    Paid = 2
}
public static class ApiKeyResolver

{
private static readonly Dictionary<string, ApiKeyTier> Keys = new(StringComparer.Ordinal)

{
     ["tms-free-demo-001"] = ApiKeyTier.Free,
     ["tms-paid-001"]   = ApiKeyTier.Paid
};
public static (string PartitionKey, ApiKeyTier Tier) Resolve(HttpContext ctx)
    {
        var key = ctx.Request.Headers["X-Api-Key"].ToString(); 
        if (string.IsNullOrEmpty(key))
        return (ctx.Connection.RemoteIpAddress?.ToString() ?? "anonymous", ApiKeyTier.Anonymous);
        return Keys.TryGetValue(key, out var tier) ? (key, tier)
        : (key, ApiKeyTier.Anonymous);

    }
}