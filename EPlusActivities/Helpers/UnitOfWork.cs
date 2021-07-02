using AutoMapper;
using EPlusActivities.API.Data;
using EPlusActivities.API.Data.Repositories;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Helpers
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IAddressRepository AddressRepository =>
            new AddressRepository(_context);
        public IWinningResultRepository WinningResultRepository =>
            new WinningResultRepository(_context);
        public IRepository<Activity> ActivityRepository =>
            new ActivityRepository(_context);
        public IRepository<Prize> PrizeRepository =>
            new PrizeRepository(_context);

    }
}