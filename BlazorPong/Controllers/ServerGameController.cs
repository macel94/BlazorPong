using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BlazorPong.Interfaces;
using BlazorPong.Shared;
using Microsoft.AspNetCore.SignalR;

namespace BlazorPong.Controllers
{
    public class ServerGameController
    {
        private readonly TimeSpan _broadcastInterval;

        private BallController _ballController;
        private readonly Broadcaster _broadcaster;
        private List<GameObject> _gameObjects;
        private Timer _broadcastLoop;

        public string Player1ConnectionId { get; set; }
        public string Player2ConnectionId { get; set; }

        public ServerGameController(IHubContext<GameHub, IBlazorPongClient> hubContext)
        {
            _broadcaster = new Broadcaster(hubContext);
            
            // We're going to broadcast to all clients a maximum of 25 times per second
            _broadcastInterval = TimeSpan.FromMilliseconds(40);
            _gameObjects = new List<GameObject>();
        }

        public void AddGameObject(string id)
        {
            if (_gameObjects.All(g => g.Id != id))
            {
                var gameObject = new GameObject { Id = id };
                _gameObjects.Add(gameObject);

                if (id == "ball")
                    _ballController = new BallController(gameObject);


                if (_gameObjects.Count == 3)
                {
                    // Now that we got both players and the ball, start the broadcast loop, and update all the GameObjects that moved
                    _broadcastLoop = new Timer(Update,
                                                null,
                                                _broadcastInterval,
                                                _broadcastInterval);
                }
            }
        }

        public void OnPlayer1Hit()
        {
            _ballController.OnPlayer1Hit();
        }

        public void OnPlayer2Hit()
        {
            _ballController.OnPlayer2Hit();
        }

        public void UpdateGameObjectPosition(GameObject clientModel)
        {
            var gameObject = _gameObjects.FirstOrDefault(g => g.Id == clientModel.Id);

            if (gameObject != null)
            {
                gameObject.Left = clientModel.Left;
                gameObject.Top = clientModel.Top;
                gameObject.LastUpdatedBy = clientModel.LastUpdatedBy;
                gameObject.Moved = true;
            }
        }

        public void UpdateGameObjectPositionsForClient(string connectionId)
        {
            _broadcaster.Broadcast(_gameObjects.Where(g => g.LastUpdatedBy != null), connectionId);
        }

        private void Update(object state)
        {
            _ballController.Update();

            _broadcaster.Broadcast(_gameObjects.Where(g => g.Moved));
        }
    }
}