using App.Tweens;
using App.Utils;
using Game.Grid;
using Game.Hexes;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Screensaver
{
    public class Screensaver : System.IDisposable
    {
        private readonly Vector3 _range = new (5, 2, 5);
        private readonly Vector3 _offset = new (0, 3, 0);
        private const float Duration = 5f;
        
        private readonly HexController _hexController;
        private readonly CinemachineCamera _camera;
        private readonly CinemachineHardLookAt _lookAt;
        private readonly Transform _aim;
        
        private ITween _tween;

        public Screensaver(HexController hexController, Transform cameraParent)
        {
            _hexController = hexController;
            _camera = cameraParent.gameObject.AddChild<CinemachineCamera>("cam_screensaver");
            _aim = cameraParent.gameObject.AddChild("cam_ScreensaverAim").transform;
            _lookAt = _camera.gameObject.AddComponent<CinemachineHardLookAt>();
            _camera.LookAt = _aim;
            _camera.Priority = 100;
            NewShot();
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            _tween?.Kill();
            Object.Destroy(_camera.gameObject);
            Object.Destroy(_aim.gameObject);
        }

        private void NewShot()
        {
            _tween?.Kill();
            _tween = CreateTween();
            _camera.Lens.FieldOfView = Random.Range(45, 90);
        }

        private HexObject GetRandomLandCell()
        {
            var iterations = 0;
            while (true)
            {
                var coord = HexGrid.GetRandomCell();
                var hexObject = _hexController.GetHexObject(coord);
                if (hexObject != null && hexObject.Height > HexFactory.WaterHeight)
                {
                    return hexObject;
                }
                iterations++;
                if (iterations > 100)
                    return hexObject;
            }
        }

        private ITween CreateTween()
        {
            var hexObject = GetRandomLandCell();
            var position = hexObject.Face.Position;
            var startPosition = RandomPositionFromCentre(position);
            var endPosition = RandomPositionFromCentre(position);

            _aim.position = position;
            _lookAt.LookAtOffset = new Vector3(Random.Range(-10, 10), 0, 0);

            return _camera.transform.TweenPosition(startPosition, endPosition, Duration)
                .SetEase(EaseType.Linear)
                .SetOnComplete(NewShot);
        }

        private Vector3 RandomPositionFromCentre(Vector3 centre)
        {
            return new Vector3(
                centre.x + Random.Range(centre.x -_range.x, centre.x + _range.x),
                centre.y + Random.Range(centre.y -_range.y, centre.y + _range.y),
                centre.z + Random.Range(centre.z -_range.z, centre.z + _range.z)
            ) + _offset;
        }
    }
}
