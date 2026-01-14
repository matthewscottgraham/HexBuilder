using System;

namespace App.Tweens
{
    public interface ITween
    {
        object Target { get; }
        string ID { get; }

        bool IsComplete { get; }
        bool IsPaused { get; }
        bool IgnoreTimeScale { get; }
        bool WasKilled { get; }
        float DelayTime { get; }

        Action OnComplete { get; }

        void Tick();
        void CompleteTween();
        void Kill();
        bool IsTargetDestroyed();
        void Pause();
        void Resume();
    }
}