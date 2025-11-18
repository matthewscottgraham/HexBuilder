using App.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace App.Utils
{
    public static class EventBusUtil
    {
        private static IReadOnlyList<Type> EventTypes {get; set;}
        private static IReadOnlyList<Type> EventBusTypes {get; set;}
        
        #if UNITY_EDITOR
        private static PlayModeStateChange PlayModeState {get; set;}
        
        [InitializeOnLoadMethod]
        public static void InitializeEditor()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            PlayModeState = state;
            if (PlayModeState == PlayModeStateChange.ExitingPlayMode)
            {
                ClearAllBusses();
            }
        }
        #endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            EventTypes = PredefinedAssemblyUtils.GetTypes(typeof(IEvent));
            EventBusTypes = InitializeAllBusses();
        }

        private static List<Type> InitializeAllBusses()
        {
            var eventBusTypes = new List<Type>();
            var typeDef = typeof(EventBus<>);
            foreach (var eventType in EventTypes)
            {
                var eventBusType = typeDef.MakeGenericType(eventType);
                eventBusTypes.Add(eventBusType);
                Debug.Log($"Initializing {eventType.Name}");
            }
            
            return eventBusTypes;
        }

        private static void ClearAllBusses()
        {
            Debug.Log("Clearing all busses");
            foreach (var busType in EventBusTypes)
            {
                var clearMethod = busType.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic);
                clearMethod?.Invoke(null, null);
            }
        }
    }
}