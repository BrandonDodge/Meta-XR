/*
 Code Artifact: GlobalEventBus.cs
 Description: Provides a typed publish-subscribe bus for HUD-wide events such as connection status changes.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-02
 Revision History:
 - 2026-04-13 - Brandon.Dodge - Update contribution ledger and Unity scripts
 - 2026-04-26 - HudLink development team - Added release prologue comments and tightened sprint-review documentation.
 Preconditions: Unity calls this artifact on the main thread; required scene references and serialized fields are assigned before runtime use.
 Acceptable Inputs: Valid Unity objects, event payloads, widget data, enum values, and inspector settings documented by the fields below.
 Unacceptable Inputs: Missing required scene references, null widget components, invalid slot choices, or sensor values outside the documented model ranges.
 Postconditions: HUD state, widget UI, event subscriptions, or diagnostic output is updated according to the public method that was called.
 Return Values: Unity lifecycle methods return void; helper methods return the type named in the method signature, or null only where the method documents a missing reference.
 Error and Exception Conditions: Unity may log errors or warnings for missing references, wrong prefab setup, invalid data, or unsupported slot assignments.
 Side Effects: May create, parent, destroy, activate, deactivate, or recolor Unity GameObjects and may publish or subscribe to HUD events.
 Invariants: Widget IDs stay stable for routing, the center safe zone remains clear, and UI updates run on Unity's main thread.
 Known Faults: Live Android bridge input is not complete yet, so several demo paths still use mock data or simulated events.
 Major Blocks: The inline comments below mark lifecycle hooks, data routing, validation, persistence, and UI update blocks.
 Line Comments: Important statements and branches carry short notes where a teammate would otherwise need extra context.
*/
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
                // Snapshot the handlers so listeners can unsubscribe during dispatch safely.
                var handlers = subscribers[eventType].ToArray();
                foreach (var handler in handlers)
                {
                    ((Action<T>)handler)?.Invoke(hudEvent);
                }
            }
        }
    }
}
