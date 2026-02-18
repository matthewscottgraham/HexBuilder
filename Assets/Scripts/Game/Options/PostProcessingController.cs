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
        
        private EventBinding<SetCameraModeEvent> _cameraModeBinding;
        private EventBinding<SetDofEvent> _setDofEvent;
        
        private void Start()
        {
            _volume = GetComponent<Volume>();
            if (_volume.profile.TryGet(out _depthOfField))
            {
                _depthOfField.active = false;
            }

            _setDofEvent = new EventBinding<SetDofEvent>(HandleSetDof);
            EventBus<SetDofEvent>.Register(_setDofEvent);

            _cameraModeBinding = new EventBinding<SetCameraModeEvent>(HandleCameraModeSet);
            EventBus<SetCameraModeEvent>.Register(_cameraModeBinding);
        }

        private void OnDestroy()
        {
            EventBus<SetDofEvent>.Deregister(_setDofEvent);
            EventBus<SetCameraModeEvent>.Deregister(_cameraModeBinding);
            _cameraModeBinding = null;
            _setDofEvent = null;
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
    }
}
