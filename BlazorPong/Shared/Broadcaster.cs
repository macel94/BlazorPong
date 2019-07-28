using System.Collections.Generic;
using BlazorPong.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace BlazorPong.Shared
{
    // This class signals the clients
    public class Broadcaster
    {
        private IHubContext<GameHub, IBlazorPongClient> _hubContext;

        public Broadcaster(IHubContext<GameHub, IBlazorPongClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public void Broadcast(IEnumerable<GameObject> gameObjectsMoved)
        {
            foreach (var gameObject in gameObjectsMoved)
            {
                // Qui si richiama su ogni client l'evento UpdateGameObjectPosition, che poi a sua volta richiamerà il metodo lato server
                _hubContext.Clients.AllExcept(gameObject.LastUpdatedBy).UpdateGameObjectPosition(gameObject);
                gameObject.Moved = false;
            }
        }

        public void Broadcast(IEnumerable<GameObject> gameObjects, string connectionId)
        {
            // Signal only the client with connectionId
            foreach (var gameObject in gameObjects)
                _hubContext.Clients.Client(connectionId).UpdateGameObjectPosition(gameObject);
        }
    }
}