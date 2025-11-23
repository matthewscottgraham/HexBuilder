using System;
using UnityEngine;

namespace App.Tweens
{
    public static class TweenExtensions
    {
        public static Tween<float> TweenSpriteAlpha(this SpriteRenderer spriteRenderer, float startAlpha,
            float endAlpha, float duration)
        {
            var id = $"{spriteRenderer.GetInstanceID()}_Alpha";
            return new Tween<float>(spriteRenderer, id, startAlpha, endAlpha, duration, value =>
            {
                var color = spriteRenderer.color;
                color.a = value;
                spriteRenderer.color = color;
            });
        }
        
        public static Tween<Vector3> TweenPosition(this Transform transform, Vector3 startPos, Vector3 endPos,
            float duration)
        {
            var id = $"{transform.gameObject.GetInstanceID()}_Position";
            return new Tween<Vector3>(transform, id, startPos, endPos, duration, value =>
            {
                transform.position = value;
            });
        }
        
        public static Tween<Vector3> TweenLocalPosition(this Transform transform, Vector3 startPos, Vector3 endPos,
            float duration)
        {
            var id = $"{transform.gameObject.GetInstanceID()}_Position";
            return new Tween<Vector3>(transform, id, startPos, endPos, duration, value =>
            {
                transform.localPosition = value;
            });
        }
        
        public static Tween<Vector3> TweenScale(this Transform transform, Vector3 startScale, Vector3 endScale,
            float duration)
        {
            var id = $"{transform.gameObject.GetInstanceID()}_Scale";
            return new Tween<Vector3>(transform, id, startScale, endScale, duration, value =>
            {
                transform.localScale = value;
            });
        }

        public static Tween<float> TweenFloat(Func<float> getFloatToTween, Action<float> setFloatToTween, float endValue,
            float duration)
        {
            var id = $"{getFloatToTween.Target.GetHashCode()}_Float";
            var target = getFloatToTween.Target;
            var startValue = getFloatToTween();
            return new Tween<float>(target, id, startValue, endValue, duration, setFloatToTween);
        }
    }
}