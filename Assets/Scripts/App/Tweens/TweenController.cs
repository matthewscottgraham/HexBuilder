using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace App.Tweens
{
    public class TweenController : MonoBehaviour, IDisposable
    {
        private readonly Dictionary<string, ITween> _tweens = new();

        private void Update()
        {
            foreach (var pair in _tweens.ToList())
            {
                var tween = pair.Value;
                tween.Tick();
                if (tween.IsComplete && !tween.WasKilled)
                {
                    if (tween.OnComplete != null)
                    {
                        tween.OnComplete.Invoke();
                        tween.OnComplete = null;
                    }

                    RemoveTween(tween.ID);
                }

                if (tween.WasKilled) RemoveTween(tween.ID);
            }
        }

        public void Dispose()
        {
            foreach (var tween in _tweens.Values.ToList()) tween.Kill();
            _tweens.Clear();
        }

        public void Initialize()
        {
            // Todo create pool
        }

        public void AddTween(ITween tween)
        {
            if (_tweens.ContainsKey(tween.ID))
            {
                _tweens[tween.ID].CompleteTween();
                RemoveTween(tween.ID);
            }

            _tweens.Add(tween.ID, tween);
        }

        private void RemoveTween(string id)
        {
            if (!_tweens.ContainsKey(id)) return;
            _tweens.Remove(id);
            // TODO add back into pool
        }
    }
}