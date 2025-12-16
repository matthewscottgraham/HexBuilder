using UnityEngine;

namespace App.Utils
{
    public static class GameObjectUtils
    {
        public static GameObject AddChild(this GameObject parentObject, string childName = "New GameObject")
        {
            var child = new GameObject(childName);
            child.transform.SetParent(parentObject.transform, false);
            child.transform.ResetLocalTransforms();
            return child;
        }

        public static T AddChild<T>(this GameObject parentObject, string childName = null) where T : Component
        {
            var child = AddChild(parentObject, childName ?? typeof(T).Name);
            var component = child.AddComponent(typeof(T));
            child.transform.ResetLocalTransforms();
            return component as T;
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (!gameObject.TryGetComponent(out T component))
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        public static Transform ResetLocalTransforms(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            return transform;
        }

        public static Transform ResetWorldTransforms(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            return transform;
        }

        public static Transform SetLocalHeight(this Transform transform, float height)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, height, transform.localPosition.z);
            return transform;
        }
    }
}