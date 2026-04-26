/*
 Code Artifact: LogEntry.kt
 Description: Formats health, GPS, and heading domain models into readable debug log rows.
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
package com.hudlink.app

import com.hudlink.core.data.model.GpsData
import com.hudlink.core.data.model.HeadingData
import com.hudlink.core.data.model.HeartRateData
import java.time.Instant
import java.time.ZoneId
import java.time.format.DateTimeFormatter
import java.util.concurrent.atomic.AtomicLong

/**
 * UI model for debug log entries displayed in RecyclerView.
 */
data class LogEntry(
    val id: Long,
    val timestamp: String,
    val type: LogType,
    val message: String
) {
    enum class LogType {
        HEART_RATE,
        GPS,
        HEADING
    }

    companion object {
        // Thread-safe ID counter
        private val idCounter = AtomicLong(0L)

        // Thread-safe date formatter (Java 8 Time API)
        private val timeFormatter: DateTimeFormatter = DateTimeFormatter
            .ofPattern("HH:mm:ss.SSS")
            .withZone(ZoneId.systemDefault())

        private fun formatTimestamp(epochMillis: Long): String =
            timeFormatter.format(Instant.ofEpochMilli(epochMillis))

        fun fromHeartRate(data: HeartRateData): LogEntry = LogEntry(
            id = idCounter.getAndIncrement(),
            timestamp = formatTimestamp(data.timestamp.epochMillis),
            type = LogType.HEART_RATE,
            message = "${data.beatsPerMinute} BPM (${data.measurementType.name.lowercase()})"
        )

        fun fromGps(data: GpsData): LogEntry = LogEntry(
            id = idCounter.getAndIncrement(),
            timestamp = formatTimestamp(data.timestamp.epochMillis),
            type = LogType.GPS,
            message = "%.4f, %.4f @ %.1f km/h".format(
                data.latitude,
                data.longitude,
                data.speedKph
            )
        )

        fun fromHeading(data: HeadingData): LogEntry = LogEntry(
            id = idCounter.getAndIncrement(),
            timestamp = formatTimestamp(data.timestamp.epochMillis),
            type = LogType.HEADING,
            message = "${data.cardinalDirection.abbreviation} (${data.azimuthDegrees.toInt()}\u00b0)"
        )
    }
}
