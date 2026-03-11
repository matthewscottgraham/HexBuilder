using System;
using UnityEngine;

namespace App.Tweens
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Vector3 axis = Vector3.up;
        [SerializeField] private float duration = 3f;
        
        private ITween _tween = null;
        
        private void OnEnable()
        {
            _tween ??= transform.TweenLocalRotationAroundAxis(axis, 360, duration)
                .SetLoops(-1)
                .SetEase(EaseType.Linear);
        }

        private void OnDisable()
        {
            _tween?.Kill();
            _tween = null;
        }
    }
}