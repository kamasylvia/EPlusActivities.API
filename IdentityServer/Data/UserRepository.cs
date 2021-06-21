using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public UserRepository(ApplicationDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<ApplicationUser> GetUserByPhoneAsync(string phoneNumber)
        {
            return await _context.Users
                .Include(p => p.PhoneNumber)
                .SingleOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(p => p.PhoneNumber)
                .ToListAsync();
        }

        public void Update(ApplicationUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}