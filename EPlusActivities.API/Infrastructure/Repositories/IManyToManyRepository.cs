using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPlusActivities.API.Infrastructure.Repositories
{
    public interface IManyToManyRepository
        <T1, T2>
        where T1 : class
        where T2 : class
    {
        Task AddAsync(T2 t2);

        void Remove(T2 t2);

        void Update(T2 t2);

        Task<IEnumerable<T2>> FindByLinkedEntityAsync(T1 t1);


        Task<bool> SaveAsync();
    }
}
