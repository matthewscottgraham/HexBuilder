using Unity.Cinemachine;
using UnityEngine;

namespace Game.Cameras
{
    [CreateAssetMenu(fileName = "CameraData", menuName = "Cameras")]
    public class CameraData : ScriptableObject
    {
        [SerializeField] private CameraType cameraType;
        [SerializeField] private CinemachineCamera prefab;
        [SerializeField] private bool useAimConstraint;

        public CameraType CameraType => cameraType;

        public CinemachineCamera CreateCamera(Transform parent)
        {
            if (prefab == null) return null;

            var instance = Instantiate(prefab, parent);

            if (!useAimConstraint) return instance;

            var aimConstraint = new GameObject($"{cameraType} Aim Constraint").transform;
            aimConstraint.SetParent(parent);
            aimConstraint.gameObject.AddComponent<CameraAimController>();

            instance.Follow = aimConstraint;

            return instance;
        }
    }
}