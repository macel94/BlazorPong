using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlazorPong.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace BlazorPong.Pages
{
    public class PongBase : ComponentBase, IDisposable
    {
        protected List<GameObject> GameObjects = new List<GameObject>();
        protected HubConnection Connection;
        protected Dictionary<string, HttpTransportType> ConnectionTypesDictionary;
        protected Enums.ClientType _playerType;
        private double _mouseOffset = 0;
        protected int Player1Points;
        protected int Player2Points;
        protected string PlayerTypeMessage;
        protected string GameMessage;
        protected string ConnectionMessage;
        private Timer _updateServerTimer;
        private HttpTransportType _connectionTypeChoice;
        [Inject]
        // ReSharper disable once MemberCanBePrivate.Global
        public IJSRuntime JsRuntime { get; set; }

        protected override void OnInitialized()
        {
            ConnectionMessage = "Please select a type of connection and click 'Connect'.";
            ConnectionTypesDictionary = new Dictionary<string, HttpTransportType>()
    {
            {
                "WebSockets", HttpTransportType.WebSockets
            },
            {
                "LongPolling", HttpTransportType.LongPolling
            },
            {
                "ServerSentEvents", HttpTransportType.ServerSentEvents
            },
        };
            _connectionTypeChoice = HttpTransportType.WebSockets;
        }

        protected async Task ConnectToHub()
        {
            await this.SetOnbeforeunload();

#if DEBUG
            // 44364 DEVELOPMENT(IIS)
            // 443 PROD o DEV BlazorPong Env(Forced Https)
            Connection = new HubConnectionBuilder()
                .WithUrl("https://localhost/gamehub", _connectionTypeChoice)
                .WithAutomaticReconnect()
                .Build();
#else
            Connection = new HubConnectionBuilder()
                .WithUrl("https://blazorpong-dev-as.azurewebsites.net/gamehub", _connectionTypeChoice)
                .WithAutomaticReconnect()
                .Build();
#endif

            Connection.On<GameObject>("UpdateGameObjectPositionOnClient", this.UpdateGameObjectPositionOnClient);
            Connection.On<Enums.ClientType, int>("UpdatePlayerPoints", this.UpdatePlayerPoints);
            Connection.On<string>("UpdateGameMessage", this.UpdateGameMessage);

            await Connection.StartAsync();

            // Ricavo che tipo di player sono(1, 2 o spettatore)
            _playerType = await Connection.InvokeAsync<Enums.ClientType>("GetClientType");

            await LogOnClient("Player type:" + _playerType.ToString());

            switch (_playerType)
            {
                case Enums.ClientType.Player1:
                    PlayerTypeMessage = "You are Player1";
                    break;
                case Enums.ClientType.Player2:
                    PlayerTypeMessage = "You are Player2";
                    break;
                default:
                    PlayerTypeMessage = "You are a Spectator";
                    break;
            }

            this.GetOrInitializeGameObjects();
            
            await LogOnClient("GameObjects initialization completed.");

            // Ogni decimo di secondo controlliamo se necessario fare l'update delle collisioni al server e in caso lo mandiamo
            // Iniziamo un secondo dopo l'inizializzazione del timer
            _updateServerTimer = new Timer(UpdateServer, null, 1000, 10);

            await LogOnClient("Timer Started!");
        }

        private async void UpdateGameMessage(string serverMessage)
        {
            GameMessage = serverMessage;
            StateHasChanged();

            // Lascio che l'utente veda il messaggio finale
            await Task.Delay(10000);

            // Lo resetto a null per mostrare il pulsante play
            GameMessage = null;

            this.StateHasChanged();
        }

        private Task UpdatePlayerPoints(Enums.ClientType clientType, int points)
        {
            switch (clientType)
            {
                case Enums.ClientType.Player1:
                    Player1Points = points;
                    GameMessage = "Player1 just made a point!";
                    break;
                case Enums.ClientType.Player2:
                    Player2Points = points;
                    GameMessage = "Player2 just made a point!";
                    break;
            }

            this.StateHasChanged();

            return Task.CompletedTask;
        }

        protected void MoveOnYAxisAndFlag(UIDragEventArgs e, GameObject go)
        {
            if (!go.Draggable)
            {
                return;
            }

            // Calcolo prima di tutto l'offset del mouse se necessario
            if ((int)_mouseOffset == 0)
            {
                _mouseOffset = e.ClientY - go.Top;
            }

            // Non devo considerare l'offset nel calcolo
            var nextTop = e.ClientY - _mouseOffset;

            // Il top può andare da un minimo di 10 a un massimo di 400(500 - altezza player)
            if (nextTop < 0)
            {
                return;
            }
            if (nextTop < 10)
            {
                nextTop = 10;
            }
            else if (nextTop > 400)
            {
                nextTop = 400;
            }

            // Ignoro l'asse x e quindi il left, che deve rimanere sempre uguale, e imposto come mosso e quindi da inviare l'oggetto
            // Di cui si è fatto il drag
            go.Top = nextTop;
            go.Moved = true;
        }

        private async void UpdateServer(object state)
        {
            if (_playerType == Enums.ClientType.Spectator)
            {
                return;
            }

            var tempGameObjects = new List<GameObject>();
            tempGameObjects.AddRange(GameObjects);

            await UpdateGameObjectPositions();

            var playerGameObject = tempGameObjects.FirstOrDefault(go => go.Id.Equals((_playerType == Enums.ClientType.Player1 ? "player1" : "player2")));

            if (playerGameObject != null)
            {
                await CheckForCollisionsWithPlayerAndBall(playerGameObject, tempGameObjects);
            }
        }

        private async Task CheckForCollisionsWithPlayerAndBall(GameObject playerGameObject, List<GameObject> tempGameObjects)
        {
            if (IsCollide(playerGameObject, tempGameObjects.FirstOrDefault(go => go.Id.Equals("ball"))))
            {
                if (_playerType == Enums.ClientType.Player1)
                {
                    await Connection.SendAsync("OnPlayer1Hit");
                }
                else
                {
                    await Connection.SendAsync("OnPlayer2Hit");
                }
            }
        }

        private bool IsCollide(GameObject gameObjectA, GameObject gameObjectB)
        {
            var aLeft = gameObjectA.Left;
            var aTop = gameObjectA.Top;
            var aWidth = gameObjectA.Width;
            var aHeight = gameObjectA.Height;
            var bLeft = gameObjectB.Left;
            var bTop = gameObjectB.Top;
            var bWidth = gameObjectB.Width;
            var bHeight = gameObjectB.Height;

            return !(
                aTop + aHeight <= bTop ||
                aTop >= bTop + bHeight ||
                aLeft + aWidth <= bLeft ||
                aLeft >= bLeft + bWidth
            );
        }

        private async Task UpdateGameObjectPositions()
        {
            foreach (var go in GameObjects)
            {
                if (go.Moved)
                {
                    await Connection.SendAsync("UpdateGameObjectPosition", go);

                    go.Moved = false;
                }
            }
        }

        private async void GetOrInitializeGameObjects()
        {
            // Chiedo al server la posizione di ogni oggetto e aspetto la risposta
            this.GameObjects = await Connection.InvokeAsync<List<GameObject>>("GetGameObjects");

            // Infine setto i draggable che non dipendono dal server
            foreach (var gameObject in GameObjects)
            {
                switch (gameObject.Id)
                {
                    case "player1":
                        gameObject.Draggable = _playerType == Enums.ClientType.Player1;
                        break;
                    case "player2":
                        gameObject.Draggable = _playerType == Enums.ClientType.Player2;
                        break;
                }
            }
        }

        /// <summary>
        /// Chiamato ogni 40 ms dal server per ridisegnare la posizione della pallina in movimento, lo sfrutto anche per ridisegnare il player
        /// </summary>
        private Task UpdateGameObjectPositionOnClient(GameObject updatedObj)
        {
            if (GameMessage != null && GameMessage != "Game started!")
            {
                GameMessage = "Game started!";
            }

            for (int i = 0; i < GameObjects.Count; i++)
            {
                var gameObject = GameObjects[i];
                if (GameObjects[i] == null || !gameObject.Id.Equals(updatedObj.Id))
                {
                    continue;
                }

                // Altrimenti lo riassegno, ci penserà blazor a ridisegnare l'elemento al prossimo hearthbeat
                GameObjects[i].Top = updatedObj.Top;
                GameObjects[i].Left = updatedObj.Left;
            }

            this.StateHasChanged();

            return Task.CompletedTask;
        }

        public async void Dispose()
        {
            // Dispose del timer
            _updateServerTimer?.Dispose();

            // Chiusura della connessione manualmente perchè l'evento non viene chiamato dal framework
            if (Connection != null)
            {
                await Connection.StopAsync();
            }

            try
            {
                await this.UnsetOnbeforeunload();
            }
            catch
            {
                //Catch silente, perchè se l'utente si disconnette e non cambia semplicemente tab, l'evento non esiste già più.
            }
        }

        private async Task SetOnbeforeunload()
        {
            await JsRuntime.InvokeAsync<object>("blazorJSPongInterop.setOnbeforeunload", DotNetObjectRef.Create(this));
        }

        private async Task UnsetOnbeforeunload()
        {
            await JsRuntime.InvokeAsync<object>("blazorJSPongInterop.unsetOnbeforeunload", DotNetObjectRef.Create(this));
        }

        /// <summary>
        /// Metodo utilizzato per loggare lato client, a scopo dimostrativo non essendoci un logger ufficialmente implementato dalla microsoft, per ora.
        /// </summary>
        private async Task LogOnClient(string message)
        {
            await JsRuntime.InvokeAsync<object>("blazorJSPongInterop.log", message);
        }

        /// <summary>
        /// Metodo invocato con cui forzo il dispose in fase di chiusura dell'applicazione
        /// </summary>
        /// <returns></returns>
        [JSInvokable]
        public Task DisposePongComponent()
        {
            Dispose();
            return Task.CompletedTask;
        }

        protected async void SetPlayerIsReady()
        {
            await Connection.SendAsync("SetPlayerIsReady");
            GameMessage = "Waiting for the other player...";
            StateHasChanged();
        }

        protected void SaveChoice(UIChangeEventArgs e)
        {
            _connectionTypeChoice = ConnectionTypesDictionary[e.Value.ToString()];
        }
    }
}
