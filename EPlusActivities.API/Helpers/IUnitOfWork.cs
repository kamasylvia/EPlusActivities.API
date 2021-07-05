using EPlusActivities.API.Data.Repositories;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Helpers
{
    public class IUnitOfWork
    {
        IAddressRepository AddressRepository { get; }
        IWinningResultRepository WinningResultRepository { get; }
        IRepository<Activity> ActivityRepository { get; }
        IRepository<Prize> PrizeRepository { get; }
    }
}