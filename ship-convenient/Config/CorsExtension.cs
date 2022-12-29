namespace ship_convenient.Config
{
    public static class CorsExtension
    {
        public static void AddCorsApp(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
                options.AddPolicy("AllowAllHeaders",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                         .AllowAnyHeader()
                        .AllowAnyMethod();
                    });

            });
        }
    }
}
