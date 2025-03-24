using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace House.API.ServiceDI
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsByEnv(this IServiceCollection services) 
        {
            services.AddCors(options =>
            {
                options.AddPolicy("Local",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:8080", "http://localhost:8080", "http://localhost:8081")// 请根据你的前端应用程序的实际地址进行修改
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
                options.AddPolicy("Development",
                    builder =>
                    {
                        builder.WithOrigins("http://103.159.207.34:8088", "https://localhost:8080", "http://localhost:8080") // 请根据你的前端应用程序的实际地址进行修改
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
                options.AddPolicy("Staging",
                    builder =>
                    {
                        builder.WithOrigins("http://103.159.207.34:8088", "https://localhost:8080", "http://localhost:8080") // 请根据你的前端应用程序的实际地址进行修改
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
#if DEBUG
                options.AddPolicy("Production",
                    builder =>
                    {
                        builder.WithOrigins("https://www.mapmarker.com.tw:80", "http://www.mapmarker.com.tw:80", "https://localhost:8080", "http://localhost:8080") // 请根据你的前端应用程序的实际地址进行修改
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
#else
                options.AddPolicy("Production",
                    builder =>
                    {
                        builder.WithOrigins("https://www.mapmarker.com.tw:80", "http://www.mapmarker.com.tw:80") // 请根据你的前端应用程序的实际地址进行修改
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
#endif
            });

            return services;
        }
    }
}
