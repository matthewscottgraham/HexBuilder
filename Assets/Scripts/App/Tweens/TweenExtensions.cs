using System;
using UnityEngine;

namespace App.Tweens
{
    public static class TweenExtensions
    {
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");

        public static Tween<float> TweenSpriteAlpha(this SpriteRenderer spriteRenderer, float startAlpha,
            float endAlpha, float duration)
        {
            return new Tween<float>(spriteRenderer, startAlpha, endAlpha, duration, value =>
            {
                var color = spriteRenderer.color;
                color.a = value;
                spriteRenderer.color = color;
            });
        }

        public static Tween<Vector3> TweenPosition(this Transform transform, Vector3 startPos, Vector3 endPos,
            float duration)
        {
            return new Tween<Vector3>(transform, startPos, endPos, duration,
                value => { if (transform) transform.position = value; });
        }

        public static Tween<Vector3> TweenLocalPosition(this Transform transform, Vector3 startPos, Vector3 endPos,
            float duration)
        {
            return new Tween<Vector3>(transform, startPos, endPos, duration,
                value => { if (transform) transform.localPosition = value; });
        }

        public static Tween<Vector3> TweenScale(this Transform transform, Vector3 startScale, Vector3 endScale,
            float duration)
        {
            return new Tween<Vector3>(transform, startScale, endScale, duration,
                value => { if (transform) transform.localScale = value; });
        }

        public static Tween<float> TweenAlpha(this Material material, float startAlpha, float endAlpha,
            float duration)
        {
            return new Tween<float>(
                material, 
                startAlpha, 
                endAlpha,
                duration, 
                value => { if (material) material.SetFloat(Alpha, value); });
        }

        public static Tween<float> TweenFloat(Func<float> getFloatToTween, Action<float> setFloatToTween,
            float endValue,
            float duration)
        {
            var target = getFloatToTween.Target;
            var startValue = getFloatToTween();
            return new Tween<float>(target, startValue, endValue, duration, setFloatToTween);
        }
    }
}