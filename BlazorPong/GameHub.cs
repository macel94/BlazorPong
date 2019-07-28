using System.Diagnostics;
using System.Threading.Tasks;
using BlazorPong.Controllers;
using BlazorPong.Interfaces;
using BlazorPong.Shared;
using Microsoft.AspNetCore.SignalR;
//using Microsoft.Extensions.Logging;

namespace BlazorPong
{
    public class GameHub : Hub<IBlazorPongClient>
    {
        //ILogger<GameHub> _logger;
        private ServerGameController _gameController;

        public void AddGameObject(string id)
        {
            _gameController.AddGameObject(id);
        }

        // TODO -FBE: Controlla se possibile gestire tramite gruppi(players e spectators) e se si può implementare un bot che gioca contro di te
        //public async Task OnBotConnected()
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, BOT_GROUP);
        //    _logger.LogInformation("Bot joined");
        //}

        //public async Task OnBotDisconnected()
        //{
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, BOT_GROUP);
        //    _logger.LogInformation("Bot left");
        //}

        //public async Task OnBotMoveReceived(string[] board, string connectionID)
        //{
        //    await Clients.Client(connectionID).SendAsync("NotifyUser", board);
        //}

        //public async Task OnUserMoveReceived(string[] board)
        //{
        //    await Clients.All.SendAsync("NotifyUser", board, Context.ConnectionId);
        //}

        public void UpdateGameObjectPosition(GameObject clientGameObject)
        {
            string playerName = Clients.Caller.PlayerName;
            Trace.TraceInformation(playerName + " moved to position " + clientGameObject.Top);
            clientGameObject.LastUpdatedBy = Context.ConnectionId;
            _gameController.UpdateGameObjectPosition(clientGameObject);
        }

        public Enums.ClientType GetClientType()
        {
            if (_gameController.Player1ConnectionId == Context.ConnectionId)
                return Enums.ClientType.Player1;

            if (_gameController.Player2ConnectionId == Context.ConnectionId)
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

        public void OnConnected()
        {
            // Keep track of who is player1/player2
            if (_gameController.Player1ConnectionId == null)
            {
                _gameController.Player1ConnectionId = Context.ConnectionId;
            }
            else if (_gameController.Player2ConnectionId == null)
            {
                _gameController.Player2ConnectionId = Context.ConnectionId;
            }

            _gameController.UpdateGameObjectPositionsForClient(Context.ConnectionId);
        }

        /// <summary>
        /// Disconnessione forzata o voluta che sia
        /// </summary>
        /// <param name="stopCalled">
        /// true, if stop was called on the client closing the connection gracefully
        /// false, if the connection has been lost
        /// </param>
        public void OnDisconnected(bool stopCalled)
        {
            if (_gameController.Player1ConnectionId == Context.ConnectionId)
            {
                _gameController.Player1ConnectionId = null;
            }
            else if (_gameController.Player2ConnectionId == Context.ConnectionId)
            {
                _gameController.Player2ConnectionId = null;
            }
        }
    }
}
