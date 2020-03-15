using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace BlazorPong.BL.Contracts.Interfaces
{
    public interface IBlazorPongClient : IHostedService
    {
        Task UpdateGameObjectPositionOnClient(GameObject gameObject);
        Task UpdatePlayerPoints(Enums.ClientType clientType, int points);
        Task UpdateGameMessage(string gameOverMessage);
    }
}
