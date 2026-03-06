using System;
using System.Collections.Generic;
using UnityEngine;

namespace HUDLink.Widgets
{
    /// <summary>
    /// Base event class for widget communication.
    /// </summary>
    public abstract class WidgetEvent 
    {
        public float Timestamp { get; private set; }
        public int Version { get; private set; }

        protected WidgetEvent(int version)
        {
            Timestamp = Time.time;
            Version = version;
        }
    }

    /// <summary>
    /// Global Event Bus for decoupled widget communication.
    /// Derived from Sprint 3 Requirement FR-3.3.
    /// </summary>
    public static class WidgetEventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> subscribers = new Dictionary<Type, List<Delegate>>();

        /// <summary>
        /// Subscribes a handler to a specific widget event type.
        /// </summary>
        public static void Subscribe<T>(Action<T> handler) where T : WidgetEvent
        {
            if (!subscribers.ContainsKey(typeof(T)))
            {
                subscribers[typeof(T)] = new List<Delegate>();
            }
            subscribers[typeof(T)].Add(handler);
        }

        /// <summary>
        /// Unsubscribes a handler from a specific widget event type.
        /// </summary>
        public static void Unsubscribe<T>(Action<T> handler) where T : WidgetEvent
        {
            if (subscribers.ContainsKey(typeof(T)))
            {
                subscribers[typeof(T)].Remove(handler);
            }
        }

        /// <summary>
        /// Publishes an event to all subscribed handlers.
        /// </summary>
        public static void Publish<T>(T widgetEvent) where T : WidgetEvent
        {
            if (subscribers.ContainsKey(typeof(T)))
            {
                // Create a copy to support iterating over modified collections if handlers unsubscribe
                var handlers = new List<Delegate>(subscribers[typeof(T)]);
                foreach (var handler in handlers)
                {
                    if (handler is Action<T> action)
                    {
                        action.Invoke(widgetEvent);
                    }
                }
            }
        }
        
        /// <summary>
        /// Clears all event subscriptions.
        /// </summary>
        public static void Clear()
        {
            subscribers.Clear();
        }
    }
}
