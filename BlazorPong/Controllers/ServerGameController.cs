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

        public void AddGameObjectOnServer(string id, GameHub gameHub)
        {
            if (GameObjects.All(g => g.Id != id))
            {
                var gameObject = new GameObject { Id = id };
                GameObjects.Add(gameObject);

                if (id == "ball")
                    BallController = new BallController(gameObject);
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

        public void UpdateGameObjectPositionOnServer(GameObject clientModel)
        {
            var gameObject = GameObjects.FirstOrDefault(g => g.Id == clientModel.Id);

            if (gameObject != null)
            {
                gameObject.Left = clientModel.Left;
                gameObject.Top = clientModel.Top;
                gameObject.LastUpdatedBy = clientModel.LastUpdatedBy;
                gameObject.Moved = true;
            }
        }
    }
}