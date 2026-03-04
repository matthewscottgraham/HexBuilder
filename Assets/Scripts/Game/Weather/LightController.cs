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
        [SerializeField] private Light sun;
        private const float Speed = 0.05f;
        private bool _isActive = true;
        private float _currentTime = 10f;
        
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
            _currentTime = setTimeEvent.Time / 24f;
            UpdateVisuals();
        }

        private void Update()
        {
            if (!_isActive) return;
            UpdateVisuals();
            _currentTime += Speed * Time.deltaTime;
            _currentTime %= 24;
        }

        private void UpdateVisuals()
        {
            var percent = _currentTime / 24f;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, percent * 360 + 180));
            
            sun.intensity = intensity.Evaluate(percent);
            sun.shadowStrength = shadowIntensity.Evaluate(percent);
            
            var temperature = Mathf.Sin(percent * Mathf.PI);
            EventBus<SetWhiteBalanceEvent>.Raise(new SetWhiteBalanceEvent(temperature));
        }
    }
}
