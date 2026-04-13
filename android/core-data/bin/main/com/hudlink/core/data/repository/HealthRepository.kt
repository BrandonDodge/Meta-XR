package com.hudlink.core.data.repository

import com.hudlink.core.data.model.HeartRateData
import kotlinx.coroutines.flow.Flow

/**
 * Repository interface for health data access.
 */
interface HealthRepository {

    /**
     * Observe real-time heart rate updates.
     * Emits new values as they become available.
     */
    fun observeHeartRate(): Flow<HeartRateData>

    /**
     * Get the most recent heart rate reading.
     */
    suspend fun getLatestHeartRate(): HeartRateData?

    /**
     * Get heart rate history within a time range.
     *
     * @param startEpochMillis Start of time range (inclusive)
     * @param endEpochMillis End of time range (inclusive)
     */
    suspend fun getHeartRateHistory(
        startEpochMillis: Long,
        endEpochMillis: Long
    ): List<HeartRateData>
}
