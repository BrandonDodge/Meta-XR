package com.hudlink.core.data.repository

import com.hudlink.core.data.model.GpsData
import com.hudlink.core.data.model.HeadingData
import kotlinx.coroutines.flow.Flow

/**
 * Repository interface for location and orientation data.
 */
interface LocationRepository {

    /**
     * Observe real-time GPS location updates.
     */
    fun observeGps(): Flow<GpsData>

    /**
     * Observe real-time device heading/orientation updates.
     */
    fun observeHeading(): Flow<HeadingData>

    /**
     * Get the most recent GPS reading.
     */
    suspend fun getLatestGps(): GpsData?

    /**
     * Get the most recent heading reading.
     */
    suspend fun getLatestHeading(): HeadingData?

    /**
     * Get GPS history within a time range.
     */
    suspend fun getGpsHistory(
        startEpochMillis: Long,
        endEpochMillis: Long
    ): List<GpsData>
}
