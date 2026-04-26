/*
 Code Artifact: HealthRepository.kt
 Description: Declares the health-data access contract used by the app and future live data sources.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-02-15
 Revision History:
 - 2026-02-15 - Zach Sevart - Very simple interface of app, basically just confirming it works.
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
package com.hudlink.core.data.repository

import com.hudlink.core.data.model.HeartRateData
import kotlinx.coroutines.flow.Flow

/**
 * Repository interface for health data access.
 */
interface HealthRepository {

    /**
     * Observe real-time heart rate updates.
     * Emits new values as they become available.
     */
    fun observeHeartRate(): Flow<HeartRateData>

    /**
     * Get the most recent heart rate reading.
     */
    suspend fun getLatestHeartRate(): HeartRateData?

    /**
     * Get heart rate history within a time range.
     *
     * @param startEpochMillis Start of time range (inclusive)
     * @param endEpochMillis End of time range (inclusive)
     */
    suspend fun getHeartRateHistory(
        startEpochMillis: Long,
        endEpochMillis: Long
    ): List<HeartRateData>
}
