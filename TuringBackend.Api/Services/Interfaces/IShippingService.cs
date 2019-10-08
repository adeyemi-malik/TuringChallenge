using System.Collections.Generic;
using System.Threading.Tasks;
using TuringBackend.Models;

namespace TuringBackend.Api.Services
{
    public interface IShippingService
    {
        Task<IEnumerable<Shipping>> GetShippingAsync();

        Task<IEnumerable<ShippingRegion>> GetShippingRegionAsync();

        Task<IEnumerable<Shipping>> GetShippingByRegionIdAsync(int id);

        Task<Shipping> GetShippingByIdAsync(int id);

        Task<ShippingRegion> GetShippingRegionIdAsync(int id);
    }
}