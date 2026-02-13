using System;
using App.Services;
using UnityEngine;

namespace App.Tweens
{
    public class Tween<T> : ITween
    {
        private readonly float _duration;
        private readonly T _endValue;

        private readonly T _startValue;
        private float _delayElapsedTime;
        private EaseType _easeType = EaseType.Linear;
        private float _elapsedTime;
        private int _loopCount = 1;

        private int _loopsCompleted;
        private Action<T> _onPercentComplete;
        private Action<T> _onTweenUpdate;
        private Action<T> _onUpdate;
        private float _percentThreshold = -1f;
        private bool _pingPong;
        private bool _reversed;

        public Tween(object target, T startValue, T endValue, float duration, Action<T> onUpdate)
        {
            Target = target;
            ID = Guid.NewGuid().ToString();
            _startValue = startValue;
            _endValue = endValue;
            _duration = duration;
            _onTweenUpdate = onUpdate;

            ServiceLocator.Instance.Get<TweenController>().AddTween(this);
        }

        public object Target { get; }
        public string ID { get; }

        public bool IsComplete { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IgnoreTimeScale { get; private set; }
        public bool WasKilled { get; private set; }
        public float DelayTime { get; private set; }

        public Action OnComplete { get; private set; }

        public void Tick()
        {
            if (IsComplete) return;
            if (IsPaused) return;
            if (IsTargetDestroyed())
            {
                Kill();
                return;
            }

            if (IgnoreTimeScale) _delayElapsedTime += Time.unscaledDeltaTime;
            else _delayElapsedTime += Time.deltaTime;

            if (_delayElapsedTime < DelayTime) return;

            if (IgnoreTimeScale) _elapsedTime += Time.unscaledDeltaTime;
            else _elapsedTime += Time.deltaTime;

            var percentage = _elapsedTime / _duration;
            var easedPercent = EaseFunctions.Ease(_easeType, percentage);

            T value;
            if (_reversed) value = Interpolate(_endValue, _startValue, easedPercent);
            else value = Interpolate(_startValue, _endValue, easedPercent);

            _onUpdate?.Invoke(value);
            _onTweenUpdate?.Invoke(value);

            if (_percentThreshold >= 0 && percentage >= _percentThreshold)
            {
                _onPercentComplete?.Invoke(value);
                _percentThreshold = -1;
            }

            if (!(_elapsedTime >= _duration)) return;
            _loopsCompleted++;
            _elapsedTime = 0f;

            if (_pingPong) _reversed = !_reversed;

            if (_loopCount <= 0 || _loopsCompleted < _loopCount) return;
            
            CompleteTween();
            IsComplete = true;
        }

        public void CompleteTween()
        {
            if (IsComplete) return;
            
            IsComplete = true;
            _onUpdate?.Invoke(_endValue);
            OnComplete?.Invoke();
            
            _onUpdate = null;
            _onTweenUpdate = null;
            _onPercentComplete = null;
            OnComplete = null;
        }

        public void Kill()
        {
            IsComplete = true;
            WasKilled = true;
            
            _onUpdate = null;
            _onTweenUpdate = null;
            _onPercentComplete = null;
            OnComplete = null;
        }

        public bool IsTargetDestroyed()
        {
            switch (Target)
            {
                case MonoBehaviour monoBehaviour when !monoBehaviour:
                case GameObject gameObject when !gameObject:
                case Delegate { Target: null }:
                    return true;
                default:
                    return false;
            }
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        public Tween<T> SetEase(EaseType easeType)
        {
            _easeType = easeType;
            return this;
        }

        public Tween<T> SetLoops(int loops, bool pingPong = false)
        {
            _loopCount = loops;
            _pingPong = pingPong;
            return this;
        }

        public ITween SetOnComplete(Action onComplete)
        {
            OnComplete = onComplete;
            return this;
        }

        public Tween<T> SetIgnoreTimeScale()
        {
            IgnoreTimeScale = true;
            return this;
        }

        public Tween<T> SetOnUpdate(Action<T> onUpdate)
        {
            _onUpdate = onUpdate;
            return this;
        }

        public Tween<T> SetOnPercentComplete(float percent, Action<T> onPercentComplete)
        {
            _percentThreshold = Mathf.Clamp01(percent);
            _onPercentComplete = onPercentComplete;
            return this;
        }

        public Tween<T> SetDelay(float delayTime)
        {
            DelayTime = delayTime;
            return this;
        }

        private static T Interpolate(T startValue, T endValue, float time)
        {
            if (startValue is float startValueFloat && endValue is float endValueFloat)
                return (T)(object)Mathf.LerpUnclamped(startValueFloat, endValueFloat, time);

            if (startValue is Vector3 startValueVector3 && endValue is Vector3 endValueVector3)
                return (T)(object)Vector3.LerpUnclamped(startValueVector3, endValueVector3, time);

            if (startValue is Vector2 startValueVector2 && endValue is Vector2 endValueVector2)
                return (T)(object)Vector2.LerpUnclamped(startValueVector2, endValueVector2, time);

            if (startValue is Color startValueColor && endValue is Color endValueColor)
                return (T)(object)Color.Lerp(startValueColor, endValueColor, time);

            throw new NotImplementedException($"Tween interpolation for type {typeof(T)} not implemented.");
        }
    }
}