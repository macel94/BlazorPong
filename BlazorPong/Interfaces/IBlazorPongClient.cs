using System.Threading.Tasks;
using BlazorPong.Shared;
using Microsoft.Extensions.Hosting;

namespace BlazorPong.Interfaces
{
    public interface IBlazorPongClient : IHostedService
    {
        Task UpdateGameObjectPositionOnClient(GameObject gameObject);
    }
}
