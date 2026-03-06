package com.hudlink.core.data.model

import java.nio.ByteBuffer

/**
 * Device orientation/heading data from sensor fusion.
 *
 * @property azimuthDegrees Compass heading in degrees (0-360, north = 0)
 * @property pitchDegrees Device tilt forward/back (-90 to 90)
 * @property rollDegrees Device tilt left/right (-180 to 180)
 * @property magneticDeclinationDegrees Correction from magnetic to true north
 */
data class HeadingData(
    override val timestamp: MetricTimestamp,
    override val source: DataSource,
    override val quality: DataQuality,
    val azimuthDegrees: Float,
    val pitchDegrees: Float = 0f,
    val rollDegrees: Float = 0f,
    val magneticDeclinationDegrees: Float = 0f
) : HealthMetric() {

    init {
        require(azimuthDegrees in 0f..360f) { "Azimuth must be between 0 and 360" }
        require(pitchDegrees in -90f..90f) { "Pitch must be between -90 and 90" }
        require(rollDegrees in -180f..180f) { "Roll must be between -180 and 180" }
    }

    override val payloadVersion: Byte = 1

    override fun toByteArray(): ByteArray {
        // Version(1) + Timestamp(8) + Azimuth(4) + Pitch(4) + Roll(4) = 21 bytes
        val buffer = ByteBuffer.allocate(21)
        buffer.put(payloadVersion)
        buffer.putLong(timestamp.epochMillis)
        buffer.putFloat(azimuthDegrees)
        buffer.putFloat(pitchDegrees)
        buffer.putFloat(rollDegrees)
        return buffer.array()
    }

    /**
     * True north heading with magnetic declination applied.
     */
    val trueNorthDegrees: Float
        get() = (azimuthDegrees + magneticDeclinationDegrees).mod(360f)

    /**
     * Cardinal direction based on azimuth.
     */
    val cardinalDirection: CardinalDirection
        get() = CardinalDirection.fromDegrees(azimuthDegrees)
}

enum class CardinalDirection(val abbreviation: String) {
    NORTH("N"),
    NORTH_EAST("NE"),
    EAST("E"),
    SOUTH_EAST("SE"),
    SOUTH("S"),
    SOUTH_WEST("SW"),
    WEST("W"),
    NORTH_WEST("NW");

    companion object {
        fun fromDegrees(degrees: Float): CardinalDirection {
            val normalized = degrees.mod(360f)
            return when {
                normalized < 22.5f || normalized >= 337.5f -> NORTH
                normalized < 67.5f -> NORTH_EAST
                normalized < 112.5f -> EAST
                normalized < 157.5f -> SOUTH_EAST
                normalized < 202.5f -> SOUTH
                normalized < 247.5f -> SOUTH_WEST
                normalized < 292.5f -> WEST
                else -> NORTH_WEST
            }
        }
    }
}
