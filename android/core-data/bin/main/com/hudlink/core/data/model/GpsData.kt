/*
 Code Artifact: GpsData.kt
 Description: Represents validated GPS readings and converts them into display and BLE-friendly forms.
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
package com.hudlink.core.data.model

import java.nio.ByteBuffer

/**
 * GPS location and motion data.
 *
 * @property latitude Latitude in degrees (-90 to 90)
 * @property longitude Longitude in degrees (-180 to 180)
 * @property altitude Altitude in meters above sea level (nullable)
 * @property speedMps Speed in meters per second
 * @property bearingDegrees Direction of travel in degrees (0-360, north = 0)
 * @property horizontalAccuracyMeters Horizontal accuracy radius in meters
 */
data class GpsData(
    override val timestamp: MetricTimestamp,
    override val source: DataSource,
    override val quality: DataQuality,
    val latitude: Double,
    val longitude: Double,
    val altitude: Double? = null,
    val speedMps: Float = 0f,
    val bearingDegrees: Float? = null,
    val horizontalAccuracyMeters: Float = 0f
) : HealthMetric() {

    init {
        require(latitude in -90.0..90.0) { "Latitude must be between -90 and 90" }
        require(longitude in -180.0..180.0) { "Longitude must be between -180 and 180" }
        require(speedMps >= 0f) { "Speed cannot be negative" }
        bearingDegrees?.let {
            require(it in 0f..360f) { "Bearing must be between 0 and 360" }
        }
    }

    override val payloadVersion: Byte = 1

    override fun toByteArray(): ByteArray {
        // Version(1) + timestamp(8) + latitude(8) + longitude(8) + altitude(8)
        // + speed(4) + bearing(4) + horizontal accuracy(4) = 45 bytes.
        val buffer = ByteBuffer.allocate(45)
        buffer.put(payloadVersion)
        buffer.putLong(timestamp.epochMillis)
        buffer.putDouble(latitude)
        buffer.putDouble(longitude)
        buffer.putDouble(altitude ?: Double.NaN)
        buffer.putFloat(speedMps)
        buffer.putFloat(bearingDegrees ?: Float.NaN)
        buffer.putFloat(horizontalAccuracyMeters)
        return buffer.array()
    }

    /**
     * Speed converted to kilometers per hour.
     */
    val speedKph: Float get() = speedMps * 3.6f

    /**
     * Speed converted to miles per hour.
     */
    val speedMph: Float get() = speedMps * 2.237f
}
