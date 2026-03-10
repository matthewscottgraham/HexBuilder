using App.Events;
using Game.Events;
using UnityEngine;

namespace Game.Weather
{
    public class TimeLimitedVisibility : MonoBehaviour
    {
        private const float OffTime = 2f;
        private const float OnTime = 17f;
        private float _randomTime = 0;
        private EventBinding<TimeUpdateEvent> _timeUpdateEventBinding;
        private MeshRenderer _meshRenderer;
        
        private void OnEnable()
        {
            _randomTime = Random.Range(-0.5f, 0.5f);
            Toggle(LightController.CurrentTime);

            _timeUpdateEventBinding ??= new EventBinding<TimeUpdateEvent>(HandleTimeUpdate);
            EventBus<TimeUpdateEvent>.Register(_timeUpdateEventBinding);
        }

        private void OnDisable()
        {
            EventBus<TimeUpdateEvent>.Deregister(_timeUpdateEventBinding);
        }

        private void HandleTimeUpdate(TimeUpdateEvent setTimeEvent)
        {
            Toggle(setTimeEvent.Time);
        }
        
        private void Toggle(float time)
        {
            if (time >= OffTime + _randomTime && time < OnTime + _randomTime)
            {
                TurnOff();
            }
            else
            {
                TurnOn();
            }
        }

        private void TurnOff()
        {
            _meshRenderer??= GetComponentInChildren<MeshRenderer>();
            _meshRenderer.enabled = false;
        }

        private void TurnOn()
        {
            _meshRenderer??= GetComponentInChildren<MeshRenderer>();
            _meshRenderer.enabled = true;
        }
    }
}
