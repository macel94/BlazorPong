using System.Threading.Tasks;
using BlazorPong.Shared;
using BlazorPongServer;
using Microsoft.Extensions.Hosting;

namespace BlazorPong.Interfaces
{
    public interface IBlazorPongClient : IHostedService
    {
        Task UpdateGameObjectPositionOnClient(GameObject gameObject);
        Task UpdatePlayerPoints(Enums.ClientType clientType, int points);
        Task UpdateGameMessage(string gameOverMessage);
    }
}
