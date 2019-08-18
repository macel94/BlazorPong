using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorPong.Controllers;
using BlazorPong.Interfaces;
using BlazorPong.Shared;
using Microsoft.AspNetCore.SignalR;

namespace BlazorPong
{
    public class GameHub : Hub<IBlazorPongClient>
    {
        private readonly ServerGameController _gameController;

        // Tramite DI
        public GameHub(ServerGameController sgc)
        {
            _gameController = sgc;
        }

        // TODO -FBE: Controlla se possibile se si può implementare un bot che gioca contro di te
        public void UpdateGameObjectPosition(GameObject clientGameObject)
        {
            // Per ora nessuna proprietà deve stare nell'interfaccia client
            //string playerName = Clients.Caller.PlayerName;
            //Trace.TraceInformation("a player" + " moved to position " + clientGameObject.Top);
            clientGameObject.LastUpdatedBy = Context.ConnectionId;
            _gameController.UpdateGameObjectPositionOnServer(clientGameObject);
        }

        public Enums.ClientType GetClientType()
        {
            if (_gameController.GetPlayer1ConnectionId() == Context.ConnectionId)
                return Enums.ClientType.Player1;

            if (_gameController.GetPlayer2ConnectionId() == Context.ConnectionId)
                return Enums.ClientType.Player2;

            return Enums.ClientType.Spectator;
        }

        public void OnPlayer1Hit()
        {
            _gameController.OnPlayer1Hit();
        }

        public void OnPlayer2Hit()
        {
            _gameController.OnPlayer2Hit();
        }

        public void SetPlayerIsReady()
        {
            if (_gameController.GetPlayer1ConnectionId() == Context.ConnectionId)
            {
                _gameController.SetPlayer1IsReady(true);
            }
            else if (_gameController.GetPlayer2ConnectionId() == Context.ConnectionId)
            {
                _gameController.SetPlayer2IsReady(true);
            }
        }

        public List<GameObject> GetGameObjects()
        {
            if (_gameController.GameObjects == null || _gameController.GameObjects.Count != 3)
            {
                // Aggiungo solo i mancanti se sono qui
                _gameController.InitializeGameObjectsOnServer(false);
            }

            return _gameController.GameObjects;
        }

        public override async Task OnConnectedAsync()
        {
            // Teniamo così traccia di chi è quale player
            if (_gameController.GetPlayer1ConnectionId() == null)
            {
                _gameController.SetPlayer1ConnectionId(Context.ConnectionId);
            }
            else if (_gameController.GetPlayer2ConnectionId() == null)
            {
                _gameController.SetPlayer2ConnectionId(Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_gameController.GetPlayer1ConnectionId() == Context.ConnectionId)
            {
                if (_gameController.MustPlayGame())
                {
                    _gameController.Player1Disconnected();
                }
                _gameController.SetPlayer1ConnectionId(null);
            }
            else if (_gameController.GetPlayer2ConnectionId() == Context.ConnectionId)
            {
                if (_gameController.MustPlayGame())
                {
                    _gameController.Player2Disconnected();
                }
                _gameController.SetPlayer2ConnectionId(null);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
