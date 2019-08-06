using System;
using System.Collections.Generic;
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
                if (_hubContext != null && _gameController.GameObjects.Count == 3)
                {
                    // Faccio sempre muovere la palla
                    _gameController.BallController.Update();

                    foreach (var gameObject in _gameController.GameObjects.Where(g => g.Moved))
                    {
                        // Qui si richiama su ogni client l'evento UpdateGameObjectPositionOnServer, che poi a sua volta richiamerà il metodo lato server
                        await _hubContext.Clients.All.UpdateGameObjectPositionOnClient(gameObject);
                        gameObject.Moved = false;
                    }
                }

                await Task.Delay(40, stoppingToken);
            }
        }
    }
}