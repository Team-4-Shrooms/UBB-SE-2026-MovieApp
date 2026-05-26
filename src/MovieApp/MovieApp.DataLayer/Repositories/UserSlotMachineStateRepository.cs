using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class UserSlotMachineStateRepository : IUserSlotMachineStateRepository
    {
        private readonly IMovieAppDbContext _context;
        public UserSlotMachineStateRepository(IMovieAppDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(UserSpinData userSpinData, CancellationToken cancellationToken = default)
        {
            System.Diagnostics.Debug.WriteLine($"Saving UserSpinData for UserId: {userSpinData.UserId}");
            _context.UserSpinData.Add(userSpinData);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserSpinData?> GetByUserIdAsync(int userIdentifier, CancellationToken cancellationToken = default)
        {
            return await _context.UserSpinData
            .FirstOrDefaultAsync(usd => usd.UserId == userIdentifier);
        }

        public async Task UpdateAsync(UserSpinData userSpinData, CancellationToken cancellationToken = default)
        {
            _context.UserSpinData.Update(userSpinData);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
