package com.hudlink.core.data.serialization

import com.hudlink.core.data.model.HealthMetric
import com.hudlink.core.data.model.HeartRateData
import org.json.JSONObject

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
        val payload = JSONObject()
        val meta = JSONObject().apply {
            put("version", VERSION)
            put("timestamp", metric.timestamp.value) // Assuming value is long
            put("source", metric.source.name)
            put("quality", metric.quality.name)
        }

        val data = JSONObject()
        when (metric) {
            is HeartRateData -> {
                payload.put("type", "HeartRate")
                data.put("bpm", metric.beatsPerMinute)
                data.put("confidence", metric.confidence)
                data.put("measurementType", metric.measurementType.name)
            }
            // other metric types like GpsData, etc.
            else -> {
                payload.put("type", "Unknown")
            }
        }

        payload.put("meta", meta)
        payload.put("data", data)
        return payload.toString()
    }
}
