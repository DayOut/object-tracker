using UserServiceRepository.Interface;
using UserServiceRepository;
using TrackedObjectServiceRepository.Interface;
using TrackedObjectServiceRepository;
using TokenServiceRepository.Interface;
using TokenServiceRepository;

namespace ObjectTrackerBackend.Extensions
{
    public static class RepositoryExtension
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITrackedObjectRepository, TrackedObjectRepository>();
            services.AddScoped<ITokenRepository, RedisTokenRepository>();
            services.AddScoped<IKeyValidator, KeyValidator>();
            return services;
        }
    }
}
