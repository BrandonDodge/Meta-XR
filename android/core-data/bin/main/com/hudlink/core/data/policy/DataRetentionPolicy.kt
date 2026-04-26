/*
 Code Artifact: DataRetentionPolicy.kt
 Description: Holds the local data retention and no-cloud-transmission policy decisions.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-02
 Revision History:
 - 2026-03-02 - Asa Maker - Implement Sprint 3 deliverables
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
package com.hudlink.core.data.policy

/**
 * Defines the rules for local data retention, ensuring privacy and compliance
 * with the Sprint 3 Requirement FR-3.7.
 */
object DataRetentionPolicy {

    /**
     * Determines whether the current metric type should be persisted to local storage at all.
     */
    fun shouldPersistDataType(dataType: String): Boolean {
        // Only specific types should be persisted for history, others might be volatile
        return when (dataType) {
            "HeartRate" -> true
            "Location" -> false // Location data is volatile and default no-persist
            else -> false
        }
    }

    /**
     * Maximum number of days a record is kept locally before automatic pruning.
     */
    const val MAX_RETENTION_DAYS = 7L

    /**
     * Represents the core rule that data should NEVER be transmitted to the cloud
     * unless explicitly enabled via an upcoming opt-in feature.
     */
    const val CLOUD_TRANSMISSION_ENABLED = false

    /**
     * Policy on when data must be securely erased immediately.
     */
    fun shouldScrubOnRevokeConsent(): Boolean {
        return true
    }
}
