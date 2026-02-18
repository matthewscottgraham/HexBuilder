using App.Events;
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
        }

        private void OnDestroy()
        {
            EventBus<SetDofEvent>.Deregister(_setDofEvent);
        }

        private void HandleSetDof(SetDofEvent evt)
        {
            _depthOfField.active = true;
            _depthOfField.focusDistance.value = Mathf.Lerp(3f, 10f, evt.Dof);
            _depthOfField.aperture.value = Mathf.Lerp(3.4f, 16f, evt.Dof);
            _depthOfField.focalLength.value = 50f;
        }
    }
}
