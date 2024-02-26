namespace Common.Auth;

public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public required string ClientId { get; set; }

    public required string ClientSecret { get; set; }

    public required string AuthServer { get; set; }

    public required string Realm { get; set; }

    public required bool ValidateAudience { get; set; } = true;

    public required IReadOnlyList<string> Audiences { get; set; }

    public int ClockSkewInSeconds { get; set; } = 5;

    public string AuthenticationScheme { get; set; } =
        JwtBearerDefaults.AuthenticationScheme;

    public string Authority => $"{AuthServer.TrimEnd('/')}/realms/{Realm.TrimStart('/')}";
}
