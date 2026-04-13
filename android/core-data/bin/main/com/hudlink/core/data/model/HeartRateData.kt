package com.hudlink.core.data.model

import java.nio.ByteBuffer

/**
 * Heart rate measurement data.
 *
 * @property beatsPerMinute Heart rate in BPM (typically 40-220)
 * @property confidence Measurement confidence from 0.0 to 1.0
 * @property measurementType Context of the measurement
 */
data class HeartRateData(
    override val timestamp: MetricTimestamp,
    override val source: DataSource,
    override val quality: DataQuality,
    val beatsPerMinute: Int,
    val confidence: Float = 1.0f,
    val measurementType: MeasurementType = MeasurementType.ACTIVE
) : HealthMetric() {

    init {
        require(beatsPerMinute in 0..300) { "BPM must be between 0 and 300" }
        require(confidence in 0f..1f) { "Confidence must be between 0.0 and 1.0" }
    }

    override val payloadVersion: Byte = 1

    override fun toByteArray(): ByteArray {
        // Version(1) + Timestamp(8) + BPM(4) + Confidence(4) + Type(1) = 18 bytes
        val buffer = ByteBuffer.allocate(18)
        buffer.put(payloadVersion)
        buffer.putLong(timestamp.epochMillis)
        buffer.putInt(beatsPerMinute)
        buffer.putFloat(confidence)
        buffer.put(measurementType.ordinal.toByte())
        return buffer.array()
    }
}

enum class MeasurementType {
    RESTING,
    ACTIVE,
    PEAK,
    RECOVERY
}
