using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace App.Services
{
    public class ServiceLocator : MonoBehaviour
    {
        private readonly Dictionary<Type, object> _services = new();
        
        public static ServiceLocator Instance { get; private set; }
        
        public void Register<T>(T service)
        {
            if (!_services.TryAdd(service.GetType(), service))
            {
                Debug.LogError($"Service type already registered: {service.GetType().Name}");
            }
        }

        public void Deregister<T>(T service)
        {
            if (service == null) return;
            var type = service.GetType();
            Deregister(type);
        }

        public void Deregister(Type type)
        {
            if (!_services.ContainsKey(type)) return;
            _services.Remove(type);
        }

        public T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return service as T;
            }
            
            throw new Exception($"Service not registered: {typeof(T).Name}");
        }

        public bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj))
            {
                service = obj as T;
                return true;
            }

            service = null;
            return false;
        }

        public void Initialize()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }
        
        public void OnDestroy()
        {
            Instance = null;
            var serviceTypes = _services.Keys.ToArray();
            foreach (var service in serviceTypes)
            {
                for (var i = 0; i < _services.Keys.Count; i++)
                {
                    Deregister(service);
                }
            }
            _services.Clear();
        }
    }
}