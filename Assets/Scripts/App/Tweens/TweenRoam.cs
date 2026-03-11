using UnityEditor;
using UnityEngine;

namespace App.Tweens
{
    public class TweenRoam : MonoBehaviour
    {
        [SerializeField] private float radius = 2f;
        [SerializeField] private float speed = 1f;
        
        private ITween _tweenPos = null;
        private ITween _tweenRot = null;
        private float _nextDelay = 0.1f;
        private void OnEnable()
        {
            ChooseNewLocation();
        }

        private void OnDisable()
        {
            _tweenPos?.Kill();
            _tweenRot?.Kill();
            _tweenPos = null;
            _tweenRot = null;
        }

        private void ChooseNewLocation()
        {
            _tweenPos?.Kill();
            _tweenRot?.Kill();
            
            var randomPos = Random.insideUnitCircle * radius;
            var newLocalPos = new Vector3(randomPos.x, 0, randomPos.y);
            var duration = Vector3.Distance(transform.localPosition, newLocalPos) / speed;
            _tweenPos = transform.TweenLocalPosition(transform.localPosition, newLocalPos, duration)
                .SetDelay(_nextDelay)
                .SetOnComplete(ChooseNewLocation);
            
            var targetWorldPos = transform.parent 
                ? transform.parent.TransformPoint(newLocalPos) 
                : newLocalPos;
            
            var direction = (targetWorldPos - transform.position).normalized;
            var targetRot = Quaternion.LookRotation(direction, Vector3.up);
            
            var localTargetRot = transform.parent
                ? Quaternion.Inverse(transform.parent.rotation) * targetRot 
                : targetRot;
            
            _tweenRot = transform.TweenLocalRotation(transform.localRotation, localTargetRot, _nextDelay / 2f).SetDelay(_nextDelay / 2f);
            
            _nextDelay = Random.Range(0f, duration * 2);
            _nextDelay = Mathf.Min(1, _nextDelay);
        }
    }
}