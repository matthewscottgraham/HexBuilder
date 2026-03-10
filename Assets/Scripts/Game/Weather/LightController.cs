using System.Collections;
using System.Collections.Generic;
using App.Events;
using Game.Events;
using UnityEngine;

namespace Game.Weather
{
    public class LightController : MonoBehaviour
    {
        private EventBinding<GamePauseEvent> _pauseBinding;
        private EventBinding<GameResumeEvent> _resumeBinding;
        private EventBinding<SetTimeEvent> _timeEventBinding;

        [SerializeField] private AnimationCurve intensity;
        [SerializeField] private AnimationCurve shadowIntensity;
        [SerializeField] private AnimationCurve whiteBalance;
        [SerializeField] private Light sun;
        private const float Speed = 0.05f;
        private bool _isActive = true;
        private float _lastSetTimeEvent = 0f;
        private static float _currentTime = 10f;
        
        public static float CurrentTime => _currentTime;
        
        private void Start()
        {
            _pauseBinding = new EventBinding<GamePauseEvent>(HandleGamePause);
            _resumeBinding = new EventBinding<GameResumeEvent>(HandleGameResume);
            _timeEventBinding = new EventBinding<SetTimeEvent>(HandleSetTime);
            EventBus<GamePauseEvent>.Register(_pauseBinding);
            EventBus<GameResumeEvent>.Register(_resumeBinding);
            EventBus<SetTimeEvent>.Register(_timeEventBinding);
        }

        private void OnDestroy()
        {
            EventBus<GamePauseEvent>.Deregister(_pauseBinding);
            EventBus<GameResumeEvent>.Deregister(_resumeBinding);
            EventBus<SetTimeEvent>.Deregister(_timeEventBinding);
        }

        private void HandleGamePause()
        {
            _isActive = false;
        }

        private void HandleGameResume()
        {
            _isActive = false;
        }

        private void HandleSetTime(SetTimeEvent setTimeEvent)
        {
            _currentTime = setTimeEvent.Time;
            _currentTime %= 24f;
            UpdateVisuals();
        }

        private void Update()
        {
            if (!_isActive) return;
            UpdateVisuals();
            _currentTime += Speed * Time.deltaTime;
            _currentTime %= 24;

            if (_currentTime > _lastSetTimeEvent + 1)
            {
                EventBus<TimeUpdateEvent>.Raise(new TimeUpdateEvent(_currentTime));
                _lastSetTimeEvent = _currentTime;
            }

            if (_lastSetTimeEvent > 24f)
            {
                _lastSetTimeEvent = 0;
            }
        }

        private void UpdateVisuals()
        {
            var percent = _currentTime / 24f;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, percent * 360 + 180));
            
            sun.intensity = intensity.Evaluate(percent);
            sun.shadowStrength = shadowIntensity.Evaluate(percent);
            
            EventBus<SetWhiteBalanceEvent>.Raise(new SetWhiteBalanceEvent(whiteBalance.Evaluate(percent)));
        }
    }
}
