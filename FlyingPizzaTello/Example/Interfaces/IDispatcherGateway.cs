using System.Threading.Tasks;
using FlyingPizzaTello;

namespace FlyingPizzaTelloTests;

public interface IDispatcherGateway
{
    public Task<bool> PutDroneState(UpdateStatusDto status);
}