using System;
using App.Events;
using App.Services;
using Game.Events;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Cameras
{
    public class CameraController : IDisposable
    {
        private readonly Camera _mainCamera;

        private CinemachineCamera _gameCamera;
        private EventBinding<SetFovEvent> _setFovEvent;

        public CameraController(Camera mainCamera)
        {
            _mainCamera = mainCamera;
            _gameCamera = mainCamera.transform.parent.GetComponentInChildren<CinemachineCamera>();
            _setFovEvent = new EventBinding<SetFovEvent>(HandleSetFov);
            EventBus<SetFovEvent>.Register(_setFovEvent);
        }

        public void Dispose()
        {
            EventBus<SetFovEvent>.Deregister(_setFovEvent);
            _setFovEvent = null;
            ServiceLocator.Instance?.Deregister(this);
        }

        private void HandleSetFov(SetFovEvent evt)
        {
            _gameCamera.Lens.FieldOfView = evt.Fov;
        }
    }
}