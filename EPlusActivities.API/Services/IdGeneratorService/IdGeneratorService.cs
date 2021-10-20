using EPlusActivities.API.Infrastructure.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Yitter.IdGenerator;

namespace EPlusActivities.API.Services.IdGeneratorService
{
    // [CustomDependency(ServiceLifetime.Singleton)]
    public class IdGeneratorService : IIdGeneratorService
    {
        private static IIdGenerator _IdGenInstance = null;

        public static IIdGenerator IdGenInstance => _IdGenInstance;

        public IdGeneratorService(IdGeneratorOptions options)
        {
            _IdGenInstance = new DefaultIdGenerator(options);
        }

        public long NextId()
        {
            if (_IdGenInstance == null)
            {
                _IdGenInstance = new DefaultIdGenerator(new IdGeneratorOptions() { WorkerId = 0 });
            }

            return _IdGenInstance.NewLong();
        }
    }
}
