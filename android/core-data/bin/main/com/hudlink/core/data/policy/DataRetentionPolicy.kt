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
