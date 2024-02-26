namespace Common.Auth;

public static class AuthExtensions
{
    /// <summary>
    /// Add JWT authentication to the service collection and configures <see cref="JwtSettings"/> options.
    /// Adds swagger security definition and requirement to use Bearer authentication.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">Configuration.</param>
    /// <returns>Service collection with added jwt authentication.</returns>
    /// <exception cref="InvalidOperationException">Throws when JwtSettings are not configured in JwtSettings.SectionName section.</exception>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettingsSection = configuration.GetRequiredSection(JwtSettings.SectionName);
        var settings = jwtSettingsSection.Get<JwtSettings>() ??
                       throw new InvalidOperationException($"JwtSettings are not configured in {JwtSettings.SectionName} section");
        _ = services.Configure<JwtSettings>(jwtSettingsSection);
        _ = services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.Authority = settings.Authority;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = settings.Authority,
                ValidateIssuerSigningKey = true,
                ValidateAudience = settings.ValidateAudience,
                ValidAudiences = settings.Audiences,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(settings.ClockSkewInSeconds),
            };
        });

        _ = services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition(
                JwtBearerDefaults.AuthenticationScheme,
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme, Id = "Bearer",
                        },
                    },
                    Array.Empty<string>()
                },
            });
        });

        return services;
    }

    /// <summary>
    /// Adds authorization policies to the service collection. Where each policy is a permission.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="permissions">List of permissions that will be added as policies.</param>
    /// <returns>Service collection with added authorization policies.</returns>
    public static IServiceCollection AddAuthorizationPolicies(
        this IServiceCollection services,
        List<string> permissions) =>
        services.AddAuthorization(options =>
        {
            foreach (string permission in permissions)
            {
                options.AddPolicy(permission, policy =>
                {
                    _ = policy.RequireAuthenticatedUser();
                    _ = policy.RequireClaim(AuthConstants.PermissionsClaimType, permission);
                });
            }
        });
}
