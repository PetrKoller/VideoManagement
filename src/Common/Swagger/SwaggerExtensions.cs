namespace Common.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddPowerTrainerSwagger(
        this IServiceCollection services,
        string serviceName) =>
        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<CommonResponseFilter>();
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = serviceName,
                Version = "v1",
                Description =
                    $"API providing interface for operations related to PowerTrainer.{serviceName}",
                Contact = new OpenApiContact { Url = new Uri("https://github.com/PetrKoller") },
            });
        });
}
