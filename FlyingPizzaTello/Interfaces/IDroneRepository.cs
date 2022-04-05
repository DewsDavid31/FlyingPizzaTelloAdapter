using System.Collections.Generic;
using System.Threading.Tasks;
using FlyingPizzaTello;

namespace FlyingPizzaTelloTests;


    public interface IDronesRepository : IBaseRepository<Drone>
    {
        public Task<Drone> GetDroneOnOrderAsync(string orderNumber);

        public Task<IEnumerable<Drone>> GetAllAvailableDronesAsync();
    }
