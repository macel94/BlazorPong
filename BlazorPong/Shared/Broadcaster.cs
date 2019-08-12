using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlazorPong.Controllers;
using BlazorPong.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace BlazorPong.Shared
{
    // This class signals the clients
    public class Broadcaster : BackgroundService
    {
        private readonly IHubContext<GameHub, IBlazorPongClient> _hubContext;
        //private readonly GameHub _hubContext;
        private readonly ServerGameController _gameController;

        public Broadcaster(IHubContext<GameHub, IBlazorPongClient> hub, ServerGameController gameController)
        {
            _hubContext = hub;
            _gameController = gameController;
        }

        //public Broadcaster(GameHub hub, ServerGameController gameController)
        //{
        //    _hubContext = hub;
        //    _gameController = gameController;
        //}

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_hubContext != null && _gameController.GameObjects.Count == 3 && _gameController.MustPlayGame())
                {
                    // Faccio sempre muovere la palla
                    _gameController.BallController.Update();

                    foreach (var gameObject in _gameController.GameObjects.Where(g => g.Moved))
                    {
                        // Se so chi ha fatto l'update evito di mandarglielo
                        if (gameObject.LastUpdatedBy != null)
                        {
                            await _hubContext.Clients.AllExcept(gameObject.LastUpdatedBy).UpdateGameObjectPositionOnClient(gameObject);
                        }
                        else
                        {
                            await _hubContext.Clients.All.UpdateGameObjectPositionOnClient(gameObject);
                        }

                        gameObject.Moved = false;
                    }
                }

                await Task.Delay(40, stoppingToken);
            }
        }
    }
}