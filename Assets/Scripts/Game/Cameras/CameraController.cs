using System;
using App.Services;
using UnityEngine;

namespace Game.Cameras
{
    public class CameraController : IDisposable
    {
        private readonly Camera _mainCamera;

        public CameraController(Camera mainCamera)
        {
            _mainCamera = mainCamera;
        }

        public void Dispose()
        {
            ServiceLocator.Instance?.Deregister(this);
        }
    }
}