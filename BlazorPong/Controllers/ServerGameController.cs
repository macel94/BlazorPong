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

        public BallController BallController;
        public List<GameObject> GameObjects;
        private string Player1ConnectionId;
        private string Player2ConnectionId;

        public ServerGameController()
        {
            GameObjects = new List<GameObject>();
        }

        public bool MustPlayGame()
        {
            return this.Player1ConnectionId != null && this.Player2ConnectionId != null;
        }

        public string GetPlayer1ConnectionId()
        {
            return this.Player1ConnectionId;
        }

        public string GetPlayer2ConnectionId()
        {
            return this.Player2ConnectionId;
        }

        public void SetPlayer1ConnectionId(string id)
        {
            this.Player1ConnectionId = id;
        }

        public void SetPlayer2ConnectionId(string id)
        {
            this.Player2ConnectionId = id;
        }

        public void AddMissingGameObjectsOnServer(List<GameObject> clientGameObjects, GameHub gameHub)
        {
            foreach (var clientGameObject in clientGameObjects)
            {
                if (GameObjects.All(g => g.Id != clientGameObject.Id))
                {
                    GameObjects.Add(clientGameObject);

                    if (clientGameObject.Id == "ball")
                        BallController = new BallController(clientGameObject);
                }
            }
        }

        public void OnPlayer1Hit()
        {
            BallController.OnPlayer1Hit();
        }

        public void OnPlayer2Hit()
        {
            BallController.OnPlayer2Hit();
        }

        public void UpdateGameObjectPositionOnServer(GameObject clientUpdatedObject)
        {
            var gameObject = GameObjects.FirstOrDefault(g => g.Id == clientUpdatedObject.Id);

            if (gameObject != null)
            {
                gameObject.Left = clientUpdatedObject.Left;
                gameObject.Top = clientUpdatedObject.Top;
                gameObject.LastUpdatedBy = clientUpdatedObject.LastUpdatedBy;
                gameObject.Moved = true;
            }
        }
    }
}