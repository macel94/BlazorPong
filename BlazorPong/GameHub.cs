using System;
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

        public void AddGameObjectOnServer(string id)
        {
            _gameController.AddGameObjectOnServer(id, this);
        }

        // TODO -FBE: Controlla se possibile se si può implementare un bot che gioca contro di te
        public void UpdateGameObjectPosition(GameObject clientGameObject)
        {
            // Per ora nessaun proprietà deve stare nell'interfaccia
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
                // TODO Fai il dispose del broadcaster
                _gameController.SetPlayer1ConnectionId(null);
            }
            else if (_gameController.GetPlayer2ConnectionId() == Context.ConnectionId)
            {
                // TODO riconnetti broadcaster se necessario
                _gameController.SetPlayer2ConnectionId(null);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
