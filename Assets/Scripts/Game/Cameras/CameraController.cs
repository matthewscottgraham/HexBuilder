using App.Services;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Cameras
{
    public class CameraController : IDisposable
    {
        private readonly Dictionary<CameraType, CinemachineCamera> _cameras = new();
        private readonly Camera _mainCamera;
        public CameraController(Camera mainCamera)
        {
            _mainCamera = mainCamera;
            CreateCameras();
            SetCameraActive(CameraType.Default);
        }

        public void Dispose()
        {
            _cameras.Clear();
            ServiceLocator.Instance?.Deregister(this);
        }

        private void CreateCameras()
        {
            var cameraData = Resources.LoadAll<CameraData>("CameraData");
            foreach (var cData in cameraData)
            {
                var camera = cData.CreateCamera(_mainCamera.transform.parent);
                if (camera == null) continue;
                _cameras.Add(cData.CameraType, camera);
            }
        }

        private void SetCameraActive(CameraType cameraType)
        {
            foreach (var key in _cameras.Keys)
            {
                _cameras[key].gameObject.SetActive(cameraType == key);
            }
        }
    }
}