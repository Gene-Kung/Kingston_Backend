using House.DAL;
using House.DAL.Dapper;
using House.Model.Enums;
using House.Service;
using Microsoft.Extensions.DependencyInjection;
using House.Model.Extensions;

namespace House.API.ServiceDI
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddHouseDao(this IServiceCollection services)
        {
            services.AddScoped<OrderDao>();
            services.AddScoped<UserDao>();
            services.AddScoped<ProductDao>();
            return services;
        }

        public static IServiceCollection AddHouseServices(this IServiceCollection services)
        {
            services.AddScoped<BaseService>();
            services.AddScoped<SecurityService>();
            services.AddScoped<OrderService>();
            services.AddScoped<UserService>();
            services.AddScoped<ProductService>();
            return services;
        }

        public static IServiceCollection AddPolicy(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(PolicyEnum.User), policy => policy.RequireRole(
                    Role.User.GetHashCodeString(),
                    Role.FreeMember.GetHashCodeString(),
                    Role.Member.GetHashCodeString(),
                    Role.VIP.GetHashCodeString(),
                    Role.Customer.GetHashCodeString(),
                    Role.SystemOperator.GetHashCodeString(),
                    Role.Admin.GetHashCodeString()));
                options.AddPolicy(nameof(PolicyEnum.FreeMember), policy => policy.RequireRole(
                    Role.FreeMember.GetHashCodeString(),
                    Role.Member.GetHashCodeString(),
                    Role.VIP.GetHashCodeString(),
                    Role.Customer.GetHashCodeString(),
                    Role.SystemOperator.GetHashCodeString(),
                    Role.Admin.GetHashCodeString()));
                options.AddPolicy(nameof(PolicyEnum.Member), policy => policy.RequireRole(
                    Role.Member.GetHashCodeString(),
                    Role.VIP.GetHashCodeString(),
                    Role.Customer.GetHashCodeString(),
                    Role.SystemOperator.GetHashCodeString(),
                    Role.Admin.GetHashCodeString()));
                options.AddPolicy(nameof(PolicyEnum.VIP), policy => policy.RequireRole(
                    Role.VIP.GetHashCodeString(),
                    Role.Customer.GetHashCodeString(),
                    Role.SystemOperator.GetHashCodeString(),
                    Role.Admin.GetHashCodeString()));
                options.AddPolicy(nameof(PolicyEnum.Customer), policy => policy.RequireRole(
                    Role.Customer.GetHashCodeString(),
                    Role.SystemOperator.GetHashCodeString(),
                    Role.Admin.GetHashCodeString()));
                options.AddPolicy(nameof(PolicyEnum.SystemOperator), policy => policy.RequireRole(
                    Role.SystemOperator.GetHashCodeString(),
                    Role.Admin.GetHashCodeString()));
                options.AddPolicy(nameof(PolicyEnum.AdminOnly), policy => policy.RequireRole(Role.Admin.GetHashCodeString()));
            });

            return services;
        }
    }
}
