using App.Events;
using Game.Events;
using UnityEngine;

namespace Game.Weather
{
    public class LightController : MonoBehaviour
    {
        private EventBinding<GamePauseEvent> _pauseBinding;
        private EventBinding<GameResumeEvent> _resumeBinding;
        private EventBinding<SetTimeOverrideEvent> _timeEventBinding;

        [SerializeField] private AnimationCurve intensity;
        [SerializeField] private AnimationCurve shadowIntensity;
        [SerializeField] private AnimationCurve whiteBalance;
        [SerializeField] private Light sun;
        private const float Speed = 0.05f;
        private bool _isActive = true;
        private int _lastSegment = -1;
        private static float _currentTime = 10f;
        
        public static float CurrentTime => _currentTime;
        
        private void Start()
        {
            _pauseBinding = new EventBinding<GamePauseEvent>(HandleGamePause);
            _resumeBinding = new EventBinding<GameResumeEvent>(HandleGameResume);
            _timeEventBinding = new EventBinding<SetTimeOverrideEvent>(HandleSetTime);
            EventBus<GamePauseEvent>.Register(_pauseBinding);
            EventBus<GameResumeEvent>.Register(_resumeBinding);
            EventBus<SetTimeOverrideEvent>.Register(_timeEventBinding);
        }

        private void OnDestroy()
        {
            EventBus<GamePauseEvent>.Deregister(_pauseBinding);
            EventBus<GameResumeEvent>.Deregister(_resumeBinding);
            EventBus<SetTimeOverrideEvent>.Deregister(_timeEventBinding);
        }

        private void HandleGamePause()
        {
            _isActive = false;
        }

        private void HandleGameResume()
        {
            _isActive = true;
        }

        private void HandleSetTime(SetTimeOverrideEvent setTimeOverrideEvent)
        {
            _currentTime = setTimeOverrideEvent.Time;
            _currentTime %= 24f;
            UpdateVisuals();
        }

        private void Update()
        {
            if (!_isActive) return;
            
            _currentTime += Speed * Time.deltaTime;
            _currentTime %= 24;
            
            UpdateVisuals();
            
            var segment = Mathf.FloorToInt(_currentTime * 12f); // every 5 minutes in game time
            if (segment == _lastSegment) return;
            
            EventBus<TimeUpdateEvent>.Raise(new TimeUpdateEvent(_currentTime));
            _lastSegment = segment;
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
