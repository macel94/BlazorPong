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
        private readonly ServerGameController _gameController;

        public Broadcaster(IHubContext<GameHub, IBlazorPongClient> hub, ServerGameController gameController)
        {
            _hubContext = hub;
            _gameController = gameController;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested && _hubContext != null)
            {
                if (_gameController.MustPlayGame())
                {
                    // Faccio sempre muovere la palla
                    var pointPlayerName = _gameController.BallController.Update();

                    // Se nessuno ha fatto punto
                    if (pointPlayerName == null)
                    {
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
                    else
                    {
                        int playerPoints;
                        Enums.ClientType playerType;
                        // Altrimenti aggiungo il punto e resetto il tutto
                        if (pointPlayerName.Equals("player1"))
                        {
                            playerPoints = _gameController.AddPlayer1Point();
                            playerType = Enums.ClientType.Player1;
                        }
                        else
                        {
                            playerPoints = _gameController.AddPlayer2Point();
                            playerType = Enums.ClientType.Player2;
                        }

                        await _hubContext.Clients.All.UpdatePlayerPoints(playerType, playerPoints);
                        // Da il tempo di visualizzare il messaggio del punto se il gioco non deve essere resettato
                        if (!_gameController.MustReset())
                        {
                            await Task.Delay(3000, stoppingToken);
                        }
                    }
                }

                if (_gameController.MustReset())
                {
                    string gameOverMessage = _gameController.GetGameOverMessage();
                    await _hubContext.Clients.All.UpdateGameMessage(gameOverMessage);
                }
                else
                {
                    await Task.Delay(40, stoppingToken);
                }
            }
        }
    }
}