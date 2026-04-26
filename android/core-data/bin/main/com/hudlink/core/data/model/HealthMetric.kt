/*
 Code Artifact: HealthMetric.kt
 Description: Defines the shared base type, source enum, and quality enum for data metrics.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-02-15
 Revision History:
 - 2026-03-06 - Asa Maker - Implement Sprint 3 HUD-Link Unity and Android core data deliverables
 - 2026-04-26 - HudLink development team - Added release prologue comments and tightened sprint-review documentation.
 Preconditions: Callers provide validated model values, Android framework objects, injected dependencies, or coroutine scopes required by the artifact.
 Acceptable Inputs: Values within the documented ranges, non-null dependencies supplied by Hilt or Android, and active coroutine collectors.
 Unacceptable Inputs: Out-of-range sensor values, missing injected dependencies, invalid Room enum names, or callbacks that ignore consent and retention rules.
 Postconditions: Domain data is validated, serialized, persisted, exposed to the UI, or emitted through repository flows as described below.
 Return Values: Methods return the domain type, Flow, List, String, ByteArray, Boolean, Unit, or nullable value stated in the signature.
 Error and Exception Conditions: Kotlin require checks throw IllegalArgumentException; Room, Hilt, coroutine, or Android APIs can throw their normal framework exceptions.
 Side Effects: May write local Room records, update SharedPreferences, emit Flow values, update UI state, or format data for the bridge.
 Invariants: Data remains local by default, timestamps remain UTC plus monotonic ordering, and consent can be revoked.
 Known Faults: Live Health Connect, fused-location, and Unity bridge integrations are planned; current streams use mock sources for the demo.
 Major Blocks: The inline comments below mark lifecycle hooks, data routing, validation, persistence, and UI update blocks.
 Line Comments: Important statements and branches carry short notes where a teammate would otherwise need extra context.
*/
package com.hudlink.core.data.model

/**
 * Base sealed class for all health metrics.
 * Provides type-safe handling of different metric types.
 */
sealed class HealthMetric : BleSerializable {
    abstract val timestamp: MetricTimestamp
    abstract val source: DataSource
    abstract val quality: DataQuality
}

/**
 * Source of the health data.
 */
enum class DataSource {
    HEALTH_CONNECT,
    DEVICE_SENSOR,
    MOCK
}

/**
 * Quality/confidence indicator for the data.
 */
enum class DataQuality {
    HIGH,
    MEDIUM,
    LOW,
    UNKNOWN
}
