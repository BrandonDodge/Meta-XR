/*
 Code Artifact: MockLocationDataSource.kt
 Description: Generates realistic mock GPS and heading readings around the demo starting location.
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

import com.hudlink.core.data.model.DataQuality
import com.hudlink.core.data.model.DataSource
import com.hudlink.core.data.model.GpsData
import com.hudlink.core.data.model.HeadingData
import com.hudlink.core.data.model.MetricTimestamp
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.flow
import javax.inject.Inject
import javax.inject.Singleton
import kotlin.math.cos
import kotlin.math.sin
import kotlin.random.Random

/**
 * Mock data source that generates realistic GPS and heading data for Sprint 2 validation.
 * Simulates movement along a path with realistic speed and heading changes.
 */
@Singleton
class MockLocationDataSource @Inject constructor() {

    // Start at University of Kansas campus (Lawrence, KS)
    private var currentLat = 38.9543
    private var currentLon = -95.2558
    private var currentBearing = 0f
    private var currentSpeed = 0f
    private var currentAzimuth = 0f

    /**
     * Emits mock GPS data at the specified interval.
     * Simulates movement with realistic position changes.
     */
    fun observeGps(intervalMs: Long = 1000L): Flow<GpsData> = flow {
        while (true) {
            emit(generateGps())
            delay(intervalMs)
        }
    }

    /**
     * Emits mock heading data at the specified interval.
     */
    fun observeHeading(intervalMs: Long = 100L): Flow<HeadingData> = flow {
        while (true) {
            emit(generateHeading())
            delay(intervalMs)
        }
    }

    /**
     * Generate a single GPS reading with simulated movement.
     */
    fun generateGps(): GpsData {
        // Simulate gradual speed changes (walking/running/stopped)
        currentSpeed = (currentSpeed + Random.nextFloat() * 2f - 1f)
            .coerceIn(0f, 5f) // 0-5 m/s (0-18 km/h, walking to jogging)

        // Simulate gradual bearing changes
        currentBearing = (currentBearing + Random.nextFloat() * 10f - 5f).mod(360f)

        // Move position based on speed and bearing
        if (currentSpeed > 0.1f) {
            val distanceMeters = currentSpeed // Distance traveled in 1 second
            val bearingRadians = Math.toRadians(currentBearing.toDouble())

            // Approximate meters to degrees (varies by latitude)
            val metersPerDegreeLat = 111320.0
            val metersPerDegreeLon = 111320.0 * cos(Math.toRadians(currentLat))

            currentLat += (distanceMeters * cos(bearingRadians)) / metersPerDegreeLat
            currentLon += (distanceMeters * sin(bearingRadians)) / metersPerDegreeLon
        }

        return GpsData(
            timestamp = MetricTimestamp.now(),
            source = DataSource.MOCK,
            quality = DataQuality.HIGH,
            latitude = currentLat,
            longitude = currentLon,
            altitude = 280.0 + Random.nextDouble() * 2.0, // ~280m elevation in Lawrence
            speedMps = currentSpeed,
            bearingDegrees = currentBearing,
            horizontalAccuracyMeters = Random.nextFloat() * 5f + 3f // 3-8m accuracy
        )
    }

    /**
     * Generate a single heading reading with device orientation.
     */
    fun generateHeading(): HeadingData {
        // Simulate gradual heading changes with some noise
        currentAzimuth = (currentAzimuth + Random.nextFloat() * 2f - 1f).mod(360f)

        return HeadingData(
            timestamp = MetricTimestamp.now(),
            source = DataSource.MOCK,
            quality = DataQuality.HIGH,
            azimuthDegrees = currentAzimuth,
            pitchDegrees = Random.nextFloat() * 10f - 5f, // -5 to 5 degrees
            rollDegrees = Random.nextFloat() * 6f - 3f,   // -3 to 3 degrees
            magneticDeclinationDegrees = 3.5f // Approximate for Kansas
        )
    }

    /**
     * Set a specific starting location for testing.
     */
    fun setLocation(latitude: Double, longitude: Double) {
        currentLat = latitude
        currentLon = longitude
    }

    /**
     * Set movement parameters for testing specific scenarios.
     */
    fun setMovement(speedMps: Float, bearingDegrees: Float) {
        currentSpeed = speedMps.coerceIn(0f, 20f)
        currentBearing = bearingDegrees.mod(360f)
    }
}
