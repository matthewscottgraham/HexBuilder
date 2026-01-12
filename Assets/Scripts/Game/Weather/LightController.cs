using App.Events;
using UnityEngine;

namespace Game.Weather
{
    public class LightController : MonoBehaviour
    {
        private EventBinding<GamePauseEvent> _pauseBinding;
        private EventBinding<GameResumeEvent> _resumeBinding;
        
        [SerializeField] private float speed = 2;
        [SerializeField] private AnimationCurve intensity;
        [SerializeField] private Light sun;
        private bool _isActive = true;
        private float _currentTime = 45f; // of 360
        
        private void Start()
        {
            _pauseBinding = new EventBinding<GamePauseEvent>(HandleGamePause);
            _resumeBinding = new EventBinding<GameResumeEvent>(HandleGameResume);
            EventBus<GamePauseEvent>.Register(_pauseBinding);
            EventBus<GameResumeEvent>.Register(_resumeBinding);
        }

        private void OnDestroy()
        {
            EventBus<GamePauseEvent>.Deregister(_pauseBinding);
            EventBus<GameResumeEvent>.Deregister(_resumeBinding);
        }

        private void HandleGamePause()
        {
            _isActive = false;
        }

        private void HandleGameResume()
        {
            _isActive = false;
        }

        private void Update()
        {
            if (!_isActive) return;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, _currentTime));
            sun.intensity = intensity.Evaluate(_currentTime / 360f);
            _currentTime -= speed * Time.deltaTime;
        }
    }
}
