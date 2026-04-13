package com.hudlink.core.data.model

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

    /**
     * Speed converted to kilometers per hour.
     */
    val speedKph: Float get() = speedMps * 3.6f

    /**
     * Speed converted to miles per hour.
     */
    val speedMph: Float get() = speedMps * 2.237f
}
