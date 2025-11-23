using Unity.Mathematics;

namespace App.Tweens
{
    public static class EaseFunctions
    {
        private const float ElasticPeriod = 0.3f;
        private const float BackConst = 1.70158f;
        private const float BackConstDouble = BackConst * 1.525f;
        
        public static float Ease(EaseType ease, float t)
        {
            switch (ease)
            {
                default:
                case EaseType.Linear: return t;

                case EaseType.QuadIn: return QuadIn(t);
                case EaseType.QuadOut: return QuadOut(t);
                case EaseType.QuadInOut: return QuadInOut(t);

                case EaseType.SineIn: return SineIn(t);
                case EaseType.SineOut: return SineOut(t);
                case EaseType.SineInOut: return SineInOut(t);

                case EaseType.CubicIn: return CubicIn(t);
                case EaseType.CubicOut: return CubicOut(t);
                case EaseType.CubicInOut: return CubicInOut(t);

                case EaseType.ElasticIn: return ElasticIn(t);
                case EaseType.ElasticOut: return ElasticOut(t);
                case EaseType.ElasticInOut: return ElasticInOut(t);

                case EaseType.BounceIn: return BounceIn(t);
                case EaseType.BounceOut: return BounceOut(t);
                case EaseType.BounceInOut: return BounceInOut(t);

                case EaseType.BackIn: return BackIn(t);
                case EaseType.BackOut: return BackOut(t);
                case EaseType.BackInOut: return BackInOut(t);
            }
        }
        
        public static float Linear(float t) => t;
        
        public static float QuadIn(float t) => t * t;
        public static float QuadOut(float t) => t * (2f - t);
        public static float QuadInOut(float t) =>
            (t < 0.5f) ? (2f * t * t) : (1f - math.pow(-2f * t + 2f, 2f) * 0.5f);
        
        public static float SineIn(float t) => 1f - math.cos(t * math.PI * 0.5f);
        public static float SineOut(float t) => math.sin(t * math.PI * 0.5f);
        public static float SineInOut(float t) => -0.5f * (math.cos(math.PI * t) - 1f);
        
        public static float CubicIn(float t) => t * t * t;
        public static float CubicOut(float t)
        {
            t -= 1f;
            return t * t * t + 1f;
        }
        public static float CubicInOut(float t)
        {
            t *= 2f;
            if (t < 1f)
                return 0.5f * t * t * t;

            t -= 2f;
            return 0.5f * (t * t * t + 2f);
        }
        
        public static float ElasticIn(float t)
        {
            if (t <= 0f || t >= 1f) return t;

            var s = ElasticPeriod * 0.25f;
            return -math.pow(2f, 10f * (t - 1f)) *
                   math.sin((t - 1f - s) * (2f * math.PI) / ElasticPeriod);
        }
        public static float ElasticOut(float t)
        {
            if (t <= 0f || t >= 1f) return t;

            float s = ElasticPeriod * 0.25f;
            return math.pow(2f, -10f * t) *
                math.sin((t - s) * (2f * math.PI) / ElasticPeriod) + 1f;
        }
        public static float ElasticInOut(float t)
        {
            if (t <= 0f || t >= 1f) return t;

            t *= 2f;
            var s = ElasticPeriod * 0.25f;

            if (t < 1f)
            {
                return -0.5f *
                       (math.pow(2f, 10f * (t - 1f)) *
                        math.sin((t - 1f - s) * (2f * math.PI) / ElasticPeriod));
            }

            t -= 1f;

            return 0.5f *
                (math.pow(2f, -10f * t) *
                 math.sin((t - s) * (2f * math.PI) / ElasticPeriod)) + 1f;
        }
        
        public static float BounceOut(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1f / d1) return n1 * t * t;

            if (t < 2f / d1)
            {
                t -= 1.5f / d1;
                return n1 * t * t + 0.75f;
            }

            if (t < 2.5f / d1)
            {
                t -= 2.25f / d1;
                return n1 * t * t + 0.9375f;
            }

            t -= 2.625f / d1;
            return n1 * t * t + 0.984375f;
        }

        public static float BounceIn(float t) => 1f - BounceOut(1f - t);
        public static float BounceInOut(float t)
        {
            if (t < 0.5f)
                return BounceIn(t * 2f) * 0.5f;
            return BounceOut(t * 2f - 1f) * 0.5f + 0.5f;
        }

        public static float BackIn(float t) => t * t * ((BackConst + 1f) * t - BackConst);
        public static float BackOut(float t)
        {
            t -= 1f;
            return t * t * ((BackConst + 1f) * t + BackConst) + 1f;
        }
        public static float BackInOut(float t)
        {
            t *= 2f;

            if (t < 1f)
                return 0.5f * (t * t * ((BackConstDouble + 1f) * t - BackConstDouble));

            t -= 2f;
            return 0.5f * (t * t * ((BackConstDouble + 1f) * t + BackConstDouble) + 2f);
        }
    }
}