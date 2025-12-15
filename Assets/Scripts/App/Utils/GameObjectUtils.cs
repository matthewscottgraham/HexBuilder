using UnityEngine;

namespace App.Utils
{
    public static class GameObjectUtils
    {
        public static GameObject AddChild(this GameObject parentObject, string childName = "New GameObject")
        {
            var child = new GameObject(childName);
            child.transform.SetParent(parentObject.transform, false);
            return child;
        }

        public static T AddChild<T>(this GameObject parentObject, string childName = null) where T : Component
        {
            var child = AddChild(parentObject, childName ?? typeof(T).Name);
            var component = child.AddComponent(typeof(T));
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
    }
}