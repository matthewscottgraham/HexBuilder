using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace App.Tweens
{
    public class TweenController : MonoBehaviour
    {
        private static TweenController _instance;
        private Dictionary<string, ITween> _tweens = new Dictionary<string, ITween>();

        public static TweenController Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("TweenController");
                    _instance = go.AddComponent<TweenController>();
                }
                return _instance;
            }
        }

        public void AddTween(ITween tween)
        {
            if (_tweens.ContainsKey(tween.ID))
                _tweens[tween.ID].CompleteTween();
            _tweens.Add(tween.ID, tween);
        }

        public void RemoveTween(string id)
        {
            if (!_tweens.ContainsKey(id)) return;
            _tweens.Remove(id);
        }

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

                if (!tween.WasKilled)
                {
                    RemoveTween(tween.ID);
                }
            }
        }
    }
}