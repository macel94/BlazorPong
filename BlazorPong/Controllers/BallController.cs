using System;
using BlazorPong.Shared;

namespace BlazorPong.Controllers
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
            _angle = 45;
        }

        private void HandleWallCollision()
        {
            if (_gameObject.Left <= LeftBounds ||
                _gameObject.Left >= RightBounds)
            {
                _lastCollisionItem = CollisionItem.Wall;
                HandleVerticalWallCollision();
            }

            if (_gameObject.Top <= BottomBounds ||
                _gameObject.Top >= TopBounds)
            {
                _lastCollisionItem = CollisionItem.Wall;
                HandleHorizontalWallCollision();
            }
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

        public void Update()
        {
            _gameObject.Left += Math.Cos(_angle * DegreeToRadians) * _speed;
            _gameObject.Top += Math.Sin(_angle * DegreeToRadians) * _speed;

            _gameObject.Moved = true;

            HandleWallCollision();
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