using System.Threading.Tasks;
using BlazorPong.Shared;
using Microsoft.AspNetCore.SignalR;

namespace BlazorPong.Interfaces
{
    public interface IBlazorPongClient : IClientProxy
    {
        string PlayerName { get; set; }
        void UpdateGameObjectPosition(GameObject gameObject);
    }
}
