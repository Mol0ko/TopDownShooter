using UnityEngine;

namespace TopDownShooter.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static T FindComponent<T>(this MonoBehaviour source)
            where T : Component
        {
            var component = source.GetComponent<T>();
            if (component == null)
            {
                Debug.LogWarning($"[{source.name}] Component {typeof(T).Name} not found");
            }
            return component;
        }

        public static T[] FindComponentsInChildren<T>(this MonoBehaviour source)
            where T : Component
        {
            var components = source.GetComponentsInChildren<T>();
            if (components == null || components.Length == 0)
            {
                Debug.LogWarning($"[{source.name}] Components {typeof(T).Name} not found");
            }
            return components;
        }
    }
}