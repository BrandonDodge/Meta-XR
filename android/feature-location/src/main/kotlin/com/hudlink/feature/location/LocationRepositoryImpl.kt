/*
 Code Artifact: LocationRepositoryImpl.kt
 Description: Connects mock GPS and heading streams to repository consumers and local storage.
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
package com.hudlink.feature.location

import com.hudlink.core.data.model.GpsData
import com.hudlink.core.data.model.HeadingData
import com.hudlink.core.data.repository.LocationRepository
import com.hudlink.core.storage.dao.GpsDao
import com.hudlink.core.storage.entity.GpsEntity
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.onEach
import kotlinx.coroutines.launch
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class LocationRepositoryImpl @Inject constructor(
    private val mockDataSource: MockLocationDataSource,
    private val gpsDao: GpsDao
) : LocationRepository {

    private val scope = CoroutineScope(SupervisorJob() + Dispatchers.IO)

    // Cache latest heading in memory (not persisted - too high frequency)
    private val latestHeading = MutableStateFlow<HeadingData?>(null)

    override fun observeGps(): Flow<GpsData> =
        mockDataSource.observeGps()
            .onEach { data ->
                // Persist GPS readings to local storage
                scope.launch {
                    gpsDao.insert(GpsEntity.fromDomain(data))
                }
            }

    override fun observeHeading(): Flow<HeadingData> =
        mockDataSource.observeHeading()
            .onEach { data ->
                // Only cache in memory - heading updates too frequent for DB
                latestHeading.value = data
            }

    override suspend fun getLatestGps(): GpsData? =
        gpsDao.getLatest()?.toDomain()

    override suspend fun getLatestHeading(): HeadingData? =
        latestHeading.value

    override suspend fun getGpsHistory(
        startEpochMillis: Long,
        endEpochMillis: Long
    ): List<GpsData> =
        gpsDao.getByTimeRange(startEpochMillis, endEpochMillis)
            .map { it.toDomain() }
}
