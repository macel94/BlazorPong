using System;
using BlazorPong.BL.Contracts;

namespace BlazorPong.BL.ServerSide.Controllers
{
    public class BallController
    {
        private enum CollisionItem
        {
            Wall = 0,
            Player1 = 1,
            Player2 = 2
        }

        private const double DegreeToRadians = Math.PI / 180;

        private const int LeftBounds = 0;
        private const int RightBounds = 1000;
        private const int BottomBounds = 0;
        private const int TopBounds = 500;

        private readonly GameObject _gameObject;
        private int _angle;
        private readonly float _speed;
        private CollisionItem _lastCollisionItem;

        public BallController(GameObject ballGameObject)
        {
            ballGameObject.Left = 500;
            ballGameObject.Top = 250;

            _gameObject = ballGameObject;
            _speed = 8f;
            var random = new Random(DateTime.Now.Millisecond);
            var next = random.Next(1, 5);
            switch (next)
            {
                case 1:
                    _angle = 45;
                    break;
                case 2:
                    _angle = 135;
                    break;
                case 3:
                    _angle = 225;
                    break;
                case 4:
                    _angle = 315;
                    break;
            }
        }

        /// <summary>
        /// Se è punto ritorna chi lo ha fatto
        /// </summary>
        /// <returns></returns>
        private string HandleWallCollision()
        {
            if (_gameObject.Left <= LeftBounds)
            {
                return "player2";
            }

            if (_gameObject.Left >= RightBounds)
            {
                return "player1";
            }

            if (_gameObject.Top <= BottomBounds ||
                _gameObject.Top >= TopBounds)
            {
                _lastCollisionItem = CollisionItem.Wall;
                HandleHorizontalWallCollision();
            }

            return null;
        }

        private void HandleVerticalWallCollision()
        {
            switch (_angle)
            {
                case 45: _angle = 135; break;
                case 135: _angle = 45; break;
                case 225: _angle = 315; break;
                case 315: _angle = 225; break;
            }
        }

        private void HandleHorizontalWallCollision()
        {
            switch (_angle)
            {
                case 45: _angle = 315; break;
                case 135: _angle = 225; break;
                case 225: _angle = 135; break;
                case 315: _angle = 45; break;
            }
        }

        public string Update()
        {
            _gameObject.Left += Math.Cos(_angle * DegreeToRadians) * _speed;
            _gameObject.Top += Math.Sin(_angle * DegreeToRadians) * _speed;

            _gameObject.Moved = true;
            
            // Se qualcuno ha fatto punto la pallina va resettata
            return HandleWallCollision();
        }

        public void OnPlayer1Hit()
        {
            if (_lastCollisionItem == CollisionItem.Player1)
                return;

            HandleVerticalWallCollision();
            _lastCollisionItem = CollisionItem.Player1;
        }

        public void OnPlayer2Hit()
        {
            if (_lastCollisionItem == CollisionItem.Player2)
                return;

            HandleVerticalWallCollision();
            _lastCollisionItem = CollisionItem.Player2;
        }
    }
}