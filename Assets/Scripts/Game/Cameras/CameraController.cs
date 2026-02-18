using System;
using App.Events;
using App.Services;
using Game.Events;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Cameras
{
    public class CameraController : IDisposable
    {
        private readonly Camera _mainCamera;

        private readonly CinemachineCamera _screenshotCamera;
        private readonly CinemachineCamera _gameCamera;

        private EventBinding<SetCameraModeEvent> _setCameraMode;
        private EventBinding<SetFovEvent> _setFovEvent;

        public CameraController(Camera mainCamera)
        {
            _mainCamera = mainCamera;

            var cameras = Object.FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
            foreach (var camera in cameras)
            {
                switch (camera.name)
                {
                    case "cam_game":
                        _gameCamera = camera;
                        break;
                    case "cam_screenshot":
                        _screenshotCamera = camera;
                        break;
                }
            }

            _setCameraMode = new EventBinding<SetCameraModeEvent>(HandleSetCameraMode);
            EventBus<SetCameraModeEvent>.Register(_setCameraMode);
            
            _setFovEvent = new EventBinding<SetFovEvent>(HandleSetFov);
            EventBus<SetFovEvent>.Register(_setFovEvent);
            
            EventBus<SetCameraModeEvent>.Raise(new SetCameraModeEvent(CameraMode.Game));
        }

        public void Dispose()
        {
            EventBus<SetFovEvent>.Deregister(_setFovEvent);
            _setFovEvent = null;
            EventBus<SetCameraModeEvent>.Deregister(_setCameraMode);
            _setCameraMode = null;
            ServiceLocator.Instance?.Deregister(this);
        }

        private void HandleSetCameraMode(SetCameraModeEvent evt)
        {
            var useScreenshotCam = evt.CameraMode == CameraMode.Screenshot;

            _screenshotCamera.Priority = useScreenshotCam ? 20 : 10;
            _gameCamera.Priority = useScreenshotCam ? 10 : 20;
        }

        private void HandleSetFov(SetFovEvent evt)
        {
            var lens = _screenshotCamera.Lens;
            lens.FieldOfView = evt.Fov;
            _screenshotCamera.Lens = lens;
        }
    }
}