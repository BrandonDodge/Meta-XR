/*
 Code Artifact: MetricSerializer.kt
 Description: Serializes metric models into versioned JSON strings for Android-to-Unity transport.
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
package com.hudlink.core.data.serialization

import com.hudlink.core.data.model.GpsData
import com.hudlink.core.data.model.HealthMetric
import com.hudlink.core.data.model.HeadingData
import com.hudlink.core.data.model.HeartRateData

/**
 * Handles the serialization and deserialization of metrics for transmission to Unity.
 * Ensures structured, versioned data models.
 * Derived from Sprint 3 Requirement FR-3.6.
 */
class MetricSerializer {

    companion object {
        const val VERSION = 1
    }

    /**
     * Serializes a generic HealthMetric into a structured JSON string wrapper for transport.
     * Incorporates versioning.
     */
    fun serialize(metric: HealthMetric): String {
        val type = metric.metricTypeName()
        val data = metric.metricDataJson()

        return """
            {
              "type": ${quote(type)},
              "meta": {
                "version": $VERSION,
                "timestamp": ${metric.timestamp.epochMillis},
                "source": ${quote(metric.source.name)},
                "quality": ${quote(metric.quality.name)}
              },
              "data": $data
            }
        """.trimIndent()
    }

    private fun HealthMetric.metricTypeName(): String = when (this) {
        is HeartRateData -> "HeartRate"
        is GpsData -> "Gps"
        is HeadingData -> "Heading"
    }

    private fun HealthMetric.metricDataJson(): String = when (this) {
        is HeartRateData -> """
            {
              "bpm": $beatsPerMinute,
              "confidence": $confidence,
              "measurementType": ${quote(measurementType.name)}
            }
        """.trimIndent()

        is GpsData -> """
            {
              "latitude": $latitude,
              "longitude": $longitude,
              "altitude": ${altitude ?: "null"},
              "speedMps": $speedMps,
              "bearingDegrees": ${bearingDegrees ?: "null"},
              "horizontalAccuracyMeters": $horizontalAccuracyMeters
            }
        """.trimIndent()

        is HeadingData -> """
            {
              "azimuthDegrees": $azimuthDegrees,
              "pitchDegrees": $pitchDegrees,
              "rollDegrees": $rollDegrees,
              "trueNorthDegrees": $trueNorthDegrees,
              "cardinalDirection": ${quote(cardinalDirection.name)}
            }
        """.trimIndent()
    }

    private fun quote(value: String): String =
        "\"" + value
            .replace("\\", "\\\\")
            .replace("\"", "\\\"") + "\""
}
