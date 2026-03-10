using App.Events;
using Game.Cameras;
using Game.Events;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.Options
{
    public class PostProcessingController : MonoBehaviour
    {
        private Volume _volume;
        private DepthOfField _depthOfField;
        private WhiteBalance _whiteBalance;
        
        private EventBinding<SetCameraModeEvent> _cameraModeBinding;
        private EventBinding<SetDofEvent> _setDofEventBinding;
        private EventBinding<SetWhiteBalanceEvent> _setWhiteBalanceEventBinding;
        
        private void Start()
        {
            _volume = GetComponent<Volume>();
            if (_volume.profile.TryGet(out _depthOfField))
            {
                _depthOfField.active = false;
            }

            if (_volume.profile.TryGet(out _whiteBalance))
            {
                _whiteBalance.active = true;
            }

            _setDofEventBinding = new EventBinding<SetDofEvent>(HandleSetDof);
            EventBus<SetDofEvent>.Register(_setDofEventBinding);

            _cameraModeBinding = new EventBinding<SetCameraModeEvent>(HandleCameraModeSet);
            EventBus<SetCameraModeEvent>.Register(_cameraModeBinding);
            
            _setWhiteBalanceEventBinding = new EventBinding<SetWhiteBalanceEvent>(HandleSetWhiteBalance);
            EventBus<SetWhiteBalanceEvent>.Register(_setWhiteBalanceEventBinding);
        }

        private void OnDestroy()
        {
            EventBus<SetDofEvent>.Deregister(_setDofEventBinding);
            EventBus<SetCameraModeEvent>.Deregister(_cameraModeBinding);
            EventBus<SetWhiteBalanceEvent>.Deregister(_setWhiteBalanceEventBinding);
            _cameraModeBinding = null;
            _setDofEventBinding = null;
            _setWhiteBalanceEventBinding = null;
        }

        private void HandleCameraModeSet(SetCameraModeEvent evt)
        {
            _depthOfField.active = evt.CameraMode == CameraMode.Screenshot;
        }

        private void HandleSetDof(SetDofEvent evt)
        {
            _depthOfField.focusDistance.value = Mathf.Lerp(3f, 10f, evt.Dof);
            _depthOfField.aperture.value = Mathf.Lerp(3.4f, 16f, evt.Dof);
            _depthOfField.focalLength.value = 50f;
        }

        private void HandleSetWhiteBalance(SetWhiteBalanceEvent evt)
        {
            _whiteBalance.temperature.value = evt.Temperature * 100;
        }
    }
}
