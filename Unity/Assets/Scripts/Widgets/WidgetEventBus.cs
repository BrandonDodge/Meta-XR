/*
 Code Artifact: WidgetEventBus.cs
 Description: Provides a typed publish-subscribe bus for communication between widgets.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-06
 Revision History:
 - 2026-04-12 - Zach Sevart - Merge test branch: add event bus, new widgets, data layer enhancements
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
using UnityEngine;

namespace HudLink.Widgets
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
