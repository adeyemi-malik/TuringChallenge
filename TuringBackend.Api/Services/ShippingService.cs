using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TuringBackend.Models;
using TuringBackend.Models.Data;

namespace TuringBackend.Api.Services
{
    public class ShippingService : IShippingService
    {
        private readonly TuringBackendContext _dbContext;

        public ShippingService(TuringBackendContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Shipping>> GetShippingAsync()
        {
            var shippings = await _dbContext
                .Shipping
                .ToListAsync();
            return shippings;
        }

        public async Task<IEnumerable<ShippingRegion>> GetShippingRegionAsync()
        {
            var shippingRegions = await _dbContext
                .ShippingRegion
                .ToListAsync();

            return shippingRegions;
        }

        public async Task<ShippingRegion> GetShippingRegionIdAsync(int id)
        {
            var shippingRegion = await _dbContext
                .ShippingRegion
                .FirstOrDefaultAsync(d => d.ShippingRegionId == id);
            return shippingRegion;
        }

        public async Task<IEnumerable<Shipping>> GetShippingByRegionIdAsync(int id)
        {
            var shipping = await _dbContext
                .Shipping
                .Where(d => d.ShippingRegionId == id).ToListAsync();
            return shipping;
        }

        public async Task<Shipping> GetShippingByIdAsync(int id)
        {
            var shipping = await _dbContext
                .Shipping
                .FirstOrDefaultAsync(d => d.ShippingId == id);
            return shipping;
        }
    }
}