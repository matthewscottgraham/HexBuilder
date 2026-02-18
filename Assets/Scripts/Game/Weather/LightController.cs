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
        
        [SerializeField] private float speed = 2;
        [SerializeField] private AnimationCurve intensity;
        [SerializeField] private Light sun;
        private bool _isActive = true;
        private static float _currentTime = 45f; // of 360
        
        public static float CurrentTime => _currentTime / 360f;
        
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
            _currentTime = setTimeEvent.Time / -24f * 360f;
            _currentTime -= 180f;
            UpdateVisuals();
        }

        private void Update()
        {
            if (!_isActive) return;
            UpdateVisuals();
            _currentTime -= speed * Time.deltaTime;
        }

        private void UpdateVisuals()
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, _currentTime));
            sun.intensity = intensity.Evaluate(_currentTime / 360f);
        }
    }
}
