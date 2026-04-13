using System;
using System.Collections.Generic;

namespace HudLink.Events
{
    /// <summary>
    /// Base class for all events in HudLink.
    /// Supports versioned payloads as per FR-3.3.
    /// </summary>
    public abstract class HUDEvent
    {
        public int Version { get; protected set; } = 1;
        public DateTime Timestamp { get; protected set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Implements publish/subscribe messaging for widgets.
    /// Derived from Sprint 3 Requirement FR-3.3.
    /// </summary>
    public static class GlobalEventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> subscribers = new Dictionary<Type, List<Delegate>>();

        /// <summary>
        /// Subscribes to an event type.
        /// </summary>
        public static void Subscribe<T>(Action<T> action) where T : HUDEvent
        {
            Type eventType = typeof(T);
            if (!subscribers.ContainsKey(eventType))
            {
                subscribers[eventType] = new List<Delegate>();
            }
            subscribers[eventType].Add(action);
        }

        /// <summary>
        /// Unsubscribes from an event type.
        /// </summary>
        public static void Unsubscribe<T>(Action<T> action) where T : HUDEvent
        {
            Type eventType = typeof(T);
            if (subscribers.ContainsKey(eventType))
            {
                subscribers[eventType].Remove(action);
            }
        }

        /// <summary>
        /// Publishes an event to all subscribers.
        /// Ensure low-latency message handling without frame-loop polling.
        /// </summary>
        public static void Publish<T>(T hudEvent) where T : HUDEvent
        {
            Type eventType = typeof(T);
            if (subscribers.ContainsKey(eventType))
            {
                // Iterate backwards to allow safe removal during invocation if necessary
                var handlers = subscribers[eventType].ToArray();
                foreach (var handler in handlers)
                {
                    ((Action<T>)handler)?.Invoke(hudEvent);
                }
            }
        }
    }
}
