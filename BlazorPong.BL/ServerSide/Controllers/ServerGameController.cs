using System.Collections.Generic;
using System.Linq;
using BlazorPong.BL.Contracts;

namespace BlazorPong.BL.ServerSide.Controllers
{
    public class ServerGameController
    {
        public BallController BallController;
        public List<GameObject> GameObjects;
        private string _player1ConnectionId;
        private string _player2ConnectionId;
        private bool _player1Ready;
        private bool _player2Ready;
        private int _player1Points;
        private int _player2Points;
        private bool _gameMustReset;

        public ServerGameController()
        {
            GameObjects = new List<GameObject>();
        }

        public bool MustReset()
        {
            return _gameMustReset;
        }

        public bool MustPlayGame()
        {
            return this.GameObjects.Count == 3
                && this._player1ConnectionId != null
                && this._player2ConnectionId != null
                && this._player1Ready
                && this._player2Ready;
        }

        public string GetPlayer1ConnectionId()
        {
            return this._player1ConnectionId;
        }

        public string GetPlayer2ConnectionId()
        {
            return this._player2ConnectionId;
        }

        public void SetPlayer1ConnectionId(string id)
        {
            this._player1ConnectionId = id;
        }

        public void SetPlayer1IsReady(bool ready)
        {
            this._player1Ready = ready;
        }

        public void SetPlayer2IsReady(bool ready)
        {
            this._player2Ready = ready;
        }

        public void SetPlayer2ConnectionId(string id)
        {
            this._player2ConnectionId = id;
        }

        /// <summary>
        /// Passando true si forza la reinizializzazione degli oggetti,
        /// false si va in aggiunta nel caso ne manchi qualcuno
        /// </summary>
        /// <param name="forceInitialization"></param>
        public void InitializeGameObjectsOnServer(bool forceInitialization)
        {
            var initializedGameObjects = new List<GameObject>()
            {
                new GameObject()
                {
                    Id = "player1",
                    LastUpdatedBy = null,
                    Left = 100,
                    Top = 100,
                    Width = 20,
                    Height = 100
                },
                new GameObject()
                {
                    Id = "player2",
                    LastUpdatedBy = null,
                    Left = 880,
                    Top = 100,
                    Width = 20,
                    Height = 100
                },
                new GameObject()
                {
                    Id = "ball",
                    LastUpdatedBy = null,
                    Left = 500,
                    Top = 250,
                    Width = 20,
                    Height = 20
                }
            };

            if (!forceInitialization)
            {
                foreach (var clientGameObject in initializedGameObjects)
                {
                    if (GameObjects.All(g => g.Id != clientGameObject.Id))
                    {
                        GameObjects.Add(clientGameObject);

                        if (clientGameObject.Id == "ball")
                            BallController = new BallController(clientGameObject);
                    }
                }
            }
            else
            {
                GameObjects = initializedGameObjects;
                BallController = new BallController(initializedGameObjects.FirstOrDefault(go => go.Id.Equals("ball")));
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

        public int AddPlayer1Point()
        {
            _player1Points++;
            InitializeGameObjectsOnServer(true);
            if (_player1Points == 3)
            {
                _gameMustReset = true;
            }
            return _player1Points;
        }

        public void Player1Disconnected()
        {
            InitializeGameObjectsOnServer(true);
            _player2Points = 3;
            _gameMustReset = true;
        }

        public int AddPlayer2Point()
        {
            _player2Points++;
            InitializeGameObjectsOnServer(true);
            if (_player2Points == 3)
            {
                _gameMustReset = true;
            }
            return _player2Points;
        }

        public void Player2Disconnected()
        {
            InitializeGameObjectsOnServer(true);
            _player1Points = 3;
            _gameMustReset = true;
        }

        public string GetGameOverMessage()
        {
            string result = null;
            // Dato che prendo il messaggio, riporto i punti a 0 come anche lo stato di player ready
            if (_player1Points == 3)
            {
                result = "Player1 won!";
            }
            else if (_player2Points == 3)
            {
                result = "Player2 won!";
            }

            _player1Points = 0;
            _player2Points = 0;
            _player2Ready = false;
            _player1Ready = false;
            _gameMustReset = false;

            return result;
        }
    }
}