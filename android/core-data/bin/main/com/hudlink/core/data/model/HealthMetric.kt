package com.hudlink.core.data.model

/**
 * Base sealed class for all health metrics.
 * Provides type-safe handling of different metric types.
 */
sealed class HealthMetric {
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
