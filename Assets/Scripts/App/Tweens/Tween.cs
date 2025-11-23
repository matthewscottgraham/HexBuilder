using System;
using App.Tweens;
using UnityEngine;

namespace App.Tweens
{
    public class Tween<T> : ITween
    {
        public Tween(object target, string propertyName, T startValue, T endValue, float duration, Action<T> onComplete)
        {
            Target = target;
            ID = propertyName;
            _startValue = startValue;
            _endValue = endValue;
            _duration = duration;
            _onTweenUpdate = onComplete;
            
            TweenController.Instance.AddTween(this);
        }
        
        private readonly T _startValue;
        private readonly T _endValue;
        private readonly float _duration;
        private float _elapsedTime = 0f;
        private float _delayElapsedTime = 0f;
        private Action<T> _onTweenUpdate;
        private EaseType _easeType = EaseType.Linear;

        private int _loopsCompleted = 0;
        private bool _reversed = false;
        private bool _pingPong = false;
        private int _loopCount = 1;
        private float _percentThreshold = -1f;
        
        private Action<T> _onUpdate;
        private Action<T> _onPercentComplete;
        
        public object Target { get; private set; }
        public string ID { get; private set;  }
        
        public bool IsComplete { get; private set;  }
        public bool IsPaused { get; private set;  }
        public bool IgnoreTimeScale { get; private set;  }
        public bool WasKilled { get; private set;  }
        public float DelayTime { get; private set;  }
        
        public Action OnComplete { get; set; }
        
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

            if (_loopCount > 0 && _loopsCompleted >= _loopCount)
            {
                CompleteTween();
                IsComplete = true;
            }
        }

        public void CompleteTween()
        {
            IsComplete = true;
            _onUpdate = null;
            _onTweenUpdate = null;
            _onPercentComplete = null;
        }

        public void Kill()
        {
            CompleteTween();
            WasKilled = true;
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

        public Tween<T> SetOnComplete(Action<T> onComplete)
        {
            _onUpdate = onComplete;
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
            {
                return (T)(object)Mathf.LerpUnclamped(startValueFloat, endValueFloat, time);
            }

            if (startValue is Vector3 startValueVector3 && endValue is Vector3 endValueVector3)
            {
                return (T)(object)Vector3.LerpUnclamped(startValueVector3, endValueVector3, time);
            }

            if (startValue is Vector2 startValueVector2 && endValue is Vector2 endValueVector2)
            {
                return (T)(object)Vector2.LerpUnclamped(startValueVector2, endValueVector2, time);
            }

            if (startValue is Color startValueColor && endValue is Color endValueColor)
            {
                return (T)(object)Color.Lerp(startValueColor, endValueColor, time);
            }
            
            throw new NotImplementedException($"Tween interpolation for type {typeof(T)} not implemented.");
        }
    }
}