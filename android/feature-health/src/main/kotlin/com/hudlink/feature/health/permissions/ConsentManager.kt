package com.hudlink.feature.health.permissions

/**
 * Manages explicit health data permissions and user consent state.
 * Derived from Sprint 3 Requirement FR-3.5.
 */
interface ConsentManager {
    
    /**
     * Checks if the user has provided consent for a specific data type.
     */
    fun hasConsent(dataType: DataType): Boolean
    
    /**
     * Requests consent for a specific category of data.
     * Implementation should trigger a UI prompt.
     */
    fun requestConsent(dataType: DataType, callback: (Boolean) -> Unit)
    
    /**
     * Revokes consent for a data category, immediately stopping data flow.
     */
    fun revokeConsent(dataType: DataType)

    /**
     * Returns a list of all current consents.
     */
    fun getActiveConsents(): List<DataType>
}

enum class DataType {
    HEART_RATE,
    LOCATION,
    NOTIFICATIONS
}
