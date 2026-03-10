using System;
using App.Events;
using Game.Events;
using UnityEngine;

namespace Game.Weather
{
    public class TimeLimitedVisibility : MonoBehaviour
    {

        private EventBinding<TimeUpdateEvent> _timeUpdateEventBinding;
        private MeshRenderer _meshRenderer;
        
        private void OnEnable()
        {
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
            if (time is > 2 and < 17) TurnOff();
            else TurnOn();
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
